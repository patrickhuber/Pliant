using Pliant.Tree;
using System;

namespace Pliant.Languages.Regex
{
    public class RegexVisitor : TreeNodeVisitorBase
    {
        public RegexDefinition Regex { get; private set; }
        
        public override void Visit(IInternalTreeNode node)
        {
            if (RegexGrammar.Regex == node.Symbol.Value)
                Regex = VisitRegexNode(node);
        }

        private RegexDefinition VisitRegexNode(IInternalTreeNode node)
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
                        switch (tokenNode.Token.Capture[0])
                        {
                            case '$':
                                endsWith = true;
                                break;

                            case '^':
                                startsWith = true;
                                break;
                        }
                        break;
                }
            }
            return new RegexDefinition(startsWith, expression, endsWith);
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
            if (term is null)
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
                    var segment = childTokenNode.Token.Capture;
                    if (segment.Count == 1 && segment[0] == '.')
                        return new RegexAtomAny();
                    continue;
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
                switch (tokenChildNode.Token.Capture[0])
                {
                    case '*':
                        return RegexIterator.ZeroOrMany;

                    case '?':
                        return RegexIterator.ZeroOrOne;

                    case '+':
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

                var isEscaped = childTokenNode.Token.Capture[0] == '\\';
                var value = isEscaped
                    ? childTokenNode.Token.Capture[1]
                    : childTokenNode.Token.Capture[0];

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
                    if (start is null)
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

                var segment = childTokenNode.Token.Capture;                
                var isEscaped = segment[0] == '\\';
                
                var value = isEscaped 
                    ? segment[1]
                    : segment[0];
                
                return new RegexCharacterClassCharacter(value, isEscaped);
            }
            throw new Exception("Invalid Regex Character Class Character.");
        }
    }
}