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

        public Regex VisitRegexNode(IInternalTreeNode node)
        {
            var regex = new Regex();

            foreach (var child in node.Children)
            {
                switch (node.NodeType)
                {
                    case TreeNodeType.Internal:

                        var internalNode = child as IInternalTreeNode;
                        switch (internalNode.Symbol.Value)
                        {
                            case "Expression":
                                regex.Expression = VisitRegexExpressionNode(internalNode);
                                break;
                        }
                        break;

                    case TreeNodeType.Token:
                        var tokenNode = child as ITokenTreeNode;
                        switch (tokenNode.Token.Value)
                        {
                            case "$":
                                regex.EndsWith = true;
                                break;
                            case "^":
                                regex.StartsWith = true;
                                break;
                        }
                        break;
                }
            }
            return regex;
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
                return new RegexExpressionAlteration
                {
                    Term = term,
                    Expression = expression
                };

            if (term != null)
                return new RegexExpressionTerm
                {
                    Term = term
                };

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
                return new RegexTerm { Factor = factor };

            return new RegexTermFactor
            {
                Term = term,
                Factor = factor
            };
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
                return new RegexFactorIterator
                {
                    Atom = atom,
                    Iterator = iterator.Value
                };

            return new RegexFactor
            {
                Atom = atom
            };
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
                        return new RegexAtomCharacter
                        {
                            Character = character
                        };

                    case "Expression":
                        var expression = VisitRegexExpressionNode(childInteralNode);
                        return new RegexAtomExpression
                        {
                            Expression = expression
                        };

                    case "Set":
                        var set = VisitRegexSetNode(childInteralNode);
                        return new RegexAtomSet
                        {
                            Set = set
                        };
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
                return new RegexCharacter
                {
                    Value = childTokenNode.Token.Value[0]
                };
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
                var set = new RegexSet();
                switch (childInternalNode.Symbol.Value)
                {
                    case "PositiveSet":
                        var positiveSet = VisitInnerSetNode(childInternalNode);
                        positiveSet.Negate = false;
                        return positiveSet;

                    case "NegativeSet":
                        var negativeSet = VisitInnerSetNode(childInternalNode);
                        negativeSet.Negate = true;
                        return negativeSet;

                }
            }
            throw new Exception("Invalid Set Detected.");
        }

        private RegexSet VisitInnerSetNode(IInternalTreeNode internalNode)
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
                        return new RegexSet
                        {
                            CharacterClass = characterClass
                        };
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
                return new RegexCharacterClassList
                {
                    CharacterClass = characterClass,
                    CharacterRange = characterRange
                };

            return new RegexCharacterClass
            {
                CharacterRange = characterRange
            };
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
                return new RegexCharacterRangeSet { StartCharacter = start, EndCharacter = end };
            return new RegexCharacterRange { StartCharacter = start };
        }

        private RegexCharacterClassCharacter VisitCharacterClassCharacterNode(IInternalTreeNode internalNode)
        {
            foreach (var child in internalNode.Children)
            {
                if (child.NodeType != TreeNodeType.Token)
                    continue;

                var childTokenNode = child as ITokenTreeNode;
                return new RegexCharacterClassCharacter
                {
                    Value = childTokenNode.Token.Value[0]
                };
            }
            throw new Exception("Invalid Regex Character Class Character.");
        }
    }
}
