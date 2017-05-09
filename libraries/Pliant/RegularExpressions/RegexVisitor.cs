using Pliant.Tree;
using System;

namespace Pliant.RegularExpressions
{
    public class RegexVisitor : TreeNodeVisitorBase
    {
        public Regex Regex { get; private set; }
        
        public override void Visit(IInternalTreeNode node)
        {
            if (RegexGrammar.Regex == node.Symbol.Value)
                Regex = VisitRegexNode(node);
        }

        private Regex VisitRegexNode(IInternalTreeNode node)
        {
            RegexExpression expression = null;
            var startsWith = false;
            var endsWith = false;

            for (var c = 0; c< node.Children.Count; c++)
            {
                var child = node.Children[c];
                switch (node.NodeType)
                {
                    case TreeNodeType.Internal:

                        var internalNode = child as IInternalTreeNode;
                        var internalNodeSymbolValue = internalNode.Symbol.Value;
                        if(RegexGrammar.Expression == internalNodeSymbolValue)
                            expression = VisitRegexExpressionNode(internalNode);
                         
                        break;

                    case TreeNodeType.Token:
                        var tokenNode = child as ITokenTreeNode;
                        switch (tokenNode.Token.Value)
                        {
                            case "$":
                                endsWith = true;
                                break;

                            case "^":
                                startsWith = true;
                                break;
                        }
                        break;
                }
            }
            return new Regex(startsWith, expression, endsWith);
        }

        private RegexExpression VisitRegexExpressionNode(IInternalTreeNode internalNode)
        {
            RegexExpression expression = null;
            RegexTerm term = null;

            for (var c = 0; c < internalNode.Children.Count; c++)
            {
                var child = internalNode.Children[c];
                if (child.NodeType != TreeNodeType.Internal)
                    continue;
                var childInternalNode = child as IInternalTreeNode;
                var childInternalNodeSymbolValue = childInternalNode.Symbol.Value;

                if (RegexGrammar.Expression == childInternalNodeSymbolValue)
                    expression = VisitRegexExpressionNode(childInternalNode);

                else if (RegexGrammar.Term == childInternalNodeSymbolValue)
                    term = VisitRegexTermNode(childInternalNode);                
            }

            if (expression != null && term != null)
                return new RegexExpressionAlteration(term, expression);

            if (term != null)
                return new RegexExpressionTerm(term);

            throw new InvalidOperationException("Unable to create null expression.");
        }

        private RegexTerm VisitRegexTermNode(IInternalTreeNode internalNode)
        {
            RegexFactor factor = null;
            RegexTerm term = null;
            for (var c = 0; c < internalNode.Children.Count; c++)
            {
                var child = internalNode.Children[c];
                if (child.NodeType != TreeNodeType.Internal)
                    continue;
                var childInternalNode = child as IInternalTreeNode;
                var childInternalNodeSymbolValue = childInternalNode.Symbol.Value;

                if (RegexGrammar.Factor == childInternalNodeSymbolValue)
                    factor = VisitRegexFactorNode(childInternalNode);
                else if (RegexGrammar.Term == childInternalNodeSymbolValue)
                    term = VisitRegexTermNode(childInternalNode);
            }
            if (term == null)
                return new RegexTerm(factor);

            return new RegexTermFactor(factor, term);
        }

        private RegexFactor VisitRegexFactorNode(IInternalTreeNode internalNode)
        {
            RegexAtom atom = null;
            RegexIterator? iterator = null;

            for (var c = 0; c < internalNode.Children.Count; c++)
            {
                var child = internalNode.Children[c];
                if (child.NodeType != TreeNodeType.Internal)
                    continue;
                var childInternalNode = child as IInternalTreeNode;
                var childInternalNodeSymbolValue = childInternalNode.Symbol.Value;

                if (RegexGrammar.Atom == childInternalNodeSymbolValue)
                    atom = VisitRegexAtomNode(childInternalNode);
                else if(RegexGrammar.Iterator == childInternalNodeSymbolValue)
                    iterator = VisitRegexIteratorNode(childInternalNode);
            }

            if (iterator.HasValue)
                return new RegexFactorIterator(
                    atom,
                    iterator.Value);

            return new RegexFactor(atom);
        }

        private RegexAtom VisitRegexAtomNode(IInternalTreeNode internalNode)
        {
            for (var c = 0; c < internalNode.Children.Count; c++)
            {
                var child = internalNode.Children[c];
                if (child.NodeType != TreeNodeType.Internal)
                {
                    var childTokenNode = child as ITokenTreeNode;
                    switch (childTokenNode.Token.Value)
                    {
                        case ".":
                            return new RegexAtomAny();

                        default:
                            continue;
                    }
                }

                var childInternalNode = child as IInternalTreeNode;
                var childInternalNodeSymbolValue = childInternalNode.Symbol.Value;
                if (RegexGrammar.Character == childInternalNodeSymbolValue)
                {
                    var character = VisitRegexCharacterNode(childInternalNode);
                    return new RegexAtomCharacter(character);
                }
                else if (RegexGrammar.Expression == childInternalNodeSymbolValue)
                {
                    var expression = VisitRegexExpressionNode(childInternalNode);
                    return new RegexAtomExpression(expression);
                }
                else if(RegexGrammar.Set == childInternalNodeSymbolValue)
                { 
                    var set = VisitRegexSetNode(childInternalNode);
                    return new RegexAtomSet(set);
                }
            }
            throw new Exception("Unable to parse atom. Invalid child production.");
        }

        private RegexIterator VisitRegexIteratorNode(IInternalTreeNode internalNode)
        {
            for (var c = 0; c < internalNode.Children.Count; c++)
            {
                var child = internalNode.Children[c];
                if (child.NodeType != TreeNodeType.Token)
                    continue;
                var tokenChildNode = child as ITokenTreeNode;
                switch (tokenChildNode.Token.Value)
                {
                    case "*":
                        return RegexIterator.ZeroOrMany;

                    case "?":
                        return RegexIterator.ZeroOrOne;

                    case "+":
                        return RegexIterator.OneOrMany;
                }
            }
            throw new Exception("Invalid iterator detected.");
        }

        private RegexCharacter VisitRegexCharacterNode(IInternalTreeNode internalNode)
        {
            for (var c = 0; c < internalNode.Children.Count; c++)
            {
                var child = internalNode.Children[c];
                if (child.NodeType != TreeNodeType.Token)
                    continue;
                var childTokenNode = child as ITokenTreeNode;

                var isEscaped = childTokenNode.Token.Value.StartsWith(@"\", StringComparison.CurrentCulture);
                var value = isEscaped
                    ? childTokenNode.Token.Value[1]
                    : childTokenNode.Token.Value[0];

                return new RegexCharacter(value, isEscaped);
            }
            throw new Exception("Invalid character detected.");
        }

        private RegexSet VisitRegexSetNode(IInternalTreeNode internalNode)
        {
            for (var c = 0; c < internalNode.Children.Count; c++)
            {
                var child = internalNode.Children[c];
                if (child.NodeType != TreeNodeType.Internal)
                    continue;

                var childInternalNode = child as IInternalTreeNode;
                var childInternalNodeSymbolValue = childInternalNode.Symbol.Value;

                if(RegexGrammar.PositiveSet == childInternalNodeSymbolValue)                
                    return VisitInnerSetNode(false, childInternalNode);

                else if(RegexGrammar.NegativeSet == childInternalNodeSymbolValue)
                    return VisitInnerSetNode(true, childInternalNode);
            }
            throw new Exception("Invalid Set Detected.");
        }

        private RegexSet VisitInnerSetNode(bool negate, IInternalTreeNode internalNode)
        {
            for (var c = 0; c < internalNode.Children.Count; c++)
            {
                var child = internalNode.Children[c];
                if (child.NodeType != TreeNodeType.Internal)
                    continue;
                var childInternalNode = child as IInternalTreeNode;
                var childInternalNodeSymbolValue = childInternalNode.Symbol.Value;
                if (RegexGrammar.CharacterClass == childInternalNodeSymbolValue)
                {
                    var characterClass = VisitCharacterClassNode(childInternalNode);
                    return new RegexSet(negate, characterClass);
                }
            }
            throw new Exception("Invalid Inner Set Detected");
        }

        private RegexCharacterClass VisitCharacterClassNode(IInternalTreeNode internalNode)
        {
            RegexCharacterUnitRange characterRange = null;
            RegexCharacterClass characterClass = null;

            for (var c = 0; c < internalNode.Children.Count; c++)
            {
                var child = internalNode.Children[c];
                if (child.NodeType != TreeNodeType.Internal)
                    continue;
                var childInternalNode = child as IInternalTreeNode;
                var childInternalNodeSymbolValue = childInternalNode.Symbol.Value;

                if (RegexGrammar.CharacterRange == childInternalNodeSymbolValue)
                    characterRange = VisitCharacterRangeNode(childInternalNode);

                else if (RegexGrammar.CharacterClass == childInternalNodeSymbolValue)
                    characterClass = VisitCharacterClassNode(childInternalNode);
                                
            }
            if (characterClass != null)
                return new RegexCharacterClassAlteration(
                    characterRange,
                    characterClass);

            return new RegexCharacterClass(characterRange);
        }

        private static RegexCharacterUnitRange VisitCharacterRangeNode(IInternalTreeNode internalNode)
        {
            RegexCharacterClassCharacter start = null;
            RegexCharacterClassCharacter end = null;

            for (var c = 0; c < internalNode.Children.Count; c++)
            {
                var child = internalNode.Children[c];
                if (child.NodeType != TreeNodeType.Internal)
                    continue;

                var childInternalNode = child as IInternalTreeNode;
                var childInternalNodeSymbolValue = childInternalNode.Symbol.Value;
                if (RegexGrammar.CharacterClassCharacter == childInternalNodeSymbolValue)
                {
                    if (start == null)
                        start = VisitCharacterClassCharacterNode(childInternalNode);
                    else
                        end = VisitCharacterClassCharacterNode(childInternalNode);
                }
            }
            if (end != null)
                return new RegexCharacterRange(start, end);

            return new RegexCharacterUnitRange(start);
        }

        private static RegexCharacterClassCharacter VisitCharacterClassCharacterNode(IInternalTreeNode internalNode)
        {
            for (var c = 0; c < internalNode.Children.Count; c++)
            {
                var child = internalNode.Children[c];
                if (child.NodeType != TreeNodeType.Token)
                    continue;

                var childTokenNode = child as ITokenTreeNode;

                var tokenValue = childTokenNode.Token.Value;
                var isEscaped = tokenValue.StartsWith(@"\", StringComparison.CurrentCulture);
                
                var value = isEscaped 
                    ? tokenValue[1]
                    : tokenValue[0];
                
                return new RegexCharacterClassCharacter(value, isEscaped);
            }
            throw new Exception("Invalid Regex Character Class Character.");
        }
    }
}