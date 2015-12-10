using Pliant.Tree;
using System;

namespace Pliant.RegularExpressions
{
    public class RegexVisitor : TreeNodeVisitorBase
    {
        public Regex Regex { get; private set; }

        public override void Visit(IInternalTreeNode node)
        {
            switch (node.Symbol.Value)
            {
                case "Regex":
                    Regex = VisitRegexNode(node);
                    break;
            }
        }

        private Regex VisitRegexNode(IInternalTreeNode node)
        {
            RegexExpression expression = null;
            bool startsWith = false;
            bool endsWith = false;

            foreach (var child in node.Children)
            {
                switch (node.NodeType)
                {
                    case TreeNodeType.Internal:

                        var internalNode = child as IInternalTreeNode;
                        switch (internalNode.Symbol.Value)
                        {
                            case "Expression":
                                expression = VisitRegexExpressionNode(internalNode);
                                break;
                        }
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
            foreach (var child in internalNode.Children)
            {
                if (child.NodeType != TreeNodeType.Internal)
                    continue;
                var childInternalNode = child as IInternalTreeNode;
                switch (childInternalNode.Symbol.Value)
                {
                    case "Expression":
                        expression = VisitRegexExpressionNode(childInternalNode);
                        break;

                    case "Term":
                        term = VisitRegexTermNode(childInternalNode);
                        break;
                }
            }

            if (expression != null && term != null)
                return new RegexExpressionAlteration(term, expression);

            if (term != null)
                return new RegexExpressionTerm(term);

            return new RegexExpression();
        }

        private RegexTerm VisitRegexTermNode(IInternalTreeNode internalNode)
        {
            RegexFactor factor = null;
            RegexTerm term = null;
            foreach (var child in internalNode.Children)
            {
                if (child.NodeType != TreeNodeType.Internal)
                    continue;
                var childInternalNode = child as IInternalTreeNode;
                switch (childInternalNode.Symbol.Value)
                {
                    case "Factor":
                        factor = VisitRegexFactorNode(childInternalNode);
                        break;

                    case "Term":
                        term = VisitRegexTermNode(childInternalNode);
                        break;
                }
            }
            if (term == null)
                return new RegexTerm(factor );

            return new RegexTermFactor(factor, term);
        }

        private RegexFactor VisitRegexFactorNode(IInternalTreeNode internalNode)
        {
            RegexAtom atom = null;
            RegexIterator? iterator = null;

            foreach (var child in internalNode.Children)
            {
                if (child.NodeType != TreeNodeType.Internal)
                    continue;
                var childInternalNode = child as IInternalTreeNode;
                switch (childInternalNode.Symbol.Value)
                {
                    case "Atom":
                        atom = VisitRegexAtomNode(childInternalNode);
                        break;

                    case "Iterator":
                        iterator = VisitRegexIteratorNode(childInternalNode);
                        break;
                }
            }

            if (iterator.HasValue)
                return new RegexFactorIterator(
                    atom, 
                    iterator.Value);

            return new RegexFactor(atom);
        }

        private RegexAtom VisitRegexAtomNode(IInternalTreeNode internalNode)
        {
            foreach (var child in internalNode.Children)
            {
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

                var childInteralNode = child as IInternalTreeNode;
                switch (childInteralNode.Symbol.Value)
                {
                    case "Character":
                        var character = VisitRegexCharacterNode(childInteralNode);
                        return new RegexAtomCharacter(character);

                    case "Expression":
                        var expression = VisitRegexExpressionNode(childInteralNode);
                        return new RegexAtomExpression(expression);

                    case "Set":
                        var set = VisitRegexSetNode(childInteralNode);
                        return new RegexAtomSet(set);
                }
            }
            throw new Exception("Unable to parse atom. Invalid child production.");
        }

        private RegexIterator VisitRegexIteratorNode(IInternalTreeNode internalNode)
        {
            foreach (var child in internalNode.Children)
            {
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
            foreach (var child in internalNode.Children)
            {
                if (child.NodeType != TreeNodeType.Token)
                    continue;
                var childTokenNode = child as ITokenTreeNode;
                var value = childTokenNode.Token.Value[0];
                return new RegexCharacter(value);
            }
            throw new Exception("Invalid character detected.");
        }

        private RegexSet VisitRegexSetNode(IInternalTreeNode internalNode)
        {
            foreach (var child in internalNode.Children)
            {
                if (child.NodeType != TreeNodeType.Internal)
                    continue;
                var childInternalNode = child as IInternalTreeNode;
                switch (childInternalNode.Symbol.Value)
                {
                    case "PositiveSet":
                        var positiveSet = VisitInnerSetNode(false, childInternalNode);
                        return positiveSet;

                    case "NegativeSet":
                        var negativeSet = VisitInnerSetNode(true, childInternalNode);
                        return negativeSet;

                }
            }
            throw new Exception("Invalid Set Detected.");
        }

        private RegexSet VisitInnerSetNode(bool negate, IInternalTreeNode internalNode)
        {
            foreach (var child in internalNode.Children)
            {
                if (child.NodeType != TreeNodeType.Internal)
                    continue;
                var childInternalNode = child as IInternalTreeNode;
                switch (childInternalNode.Symbol.Value)
                {
                    case "CharacterClass":
                        var characterClass = VisitCharacterClassNode(childInternalNode);
                        return new RegexSet(negate, characterClass);
                }
            }
            throw new Exception("Invalid Inner Set Detected");
        }

        private RegexCharacterClass VisitCharacterClassNode(IInternalTreeNode internalNode)
        {
            RegexCharacterRange characterRange = null;
            RegexCharacterClass characterClass = null;

            foreach (var child in internalNode.Children)
            {
                if (child.NodeType != TreeNodeType.Internal)
                    continue;
                var childInternalNode = child as IInternalTreeNode;
                switch (childInternalNode.Symbol.Value)
                {
                    case "CharacterRange":
                        characterRange = VisitCharacterRangeNode(childInternalNode);
                        break;
                    case "CharacterClass":
                        characterClass = VisitCharacterClassNode(childInternalNode);
                        break;
                }
            }
            if (characterClass != null)
                return new RegexCharacterClassList(
                    characterRange, 
                    characterClass);

            return new RegexCharacterClass(characterRange);
        }

        private RegexCharacterRange VisitCharacterRangeNode(IInternalTreeNode internalNode)
        {
            RegexCharacterClassCharacter start = null;
            RegexCharacterClassCharacter end = null;

            foreach (var child in internalNode.Children)
            {
                if (child.NodeType != TreeNodeType.Internal)
                    continue;

                var childInternalNode = child as IInternalTreeNode;
                switch (childInternalNode.Symbol.Value)
                {
                    case "CharacterClassCharacter":
                        if (start == null)
                            start = VisitCharacterClassCharacterNode(childInternalNode);
                        else
                            end = VisitCharacterClassCharacterNode(childInternalNode);
                        break;
                }
            }
            if (end != null)
                return new RegexCharacterRangeSet(start, end);

            return new RegexCharacterRange(start);
        }

        private RegexCharacterClassCharacter VisitCharacterClassCharacterNode(IInternalTreeNode internalNode)
        {
            foreach (var child in internalNode.Children)
            {
                if (child.NodeType != TreeNodeType.Token)
                    continue;

                var childTokenNode = child as ITokenTreeNode;
                var value = childTokenNode.Token.Value[0];
                return new RegexCharacterClassCharacter(value);
            }
            throw new Exception("Invalid Regex Character Class Character.");
        }
    }
}
