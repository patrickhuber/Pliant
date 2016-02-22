using System;
using Pliant.Tree;
using Pliant.RegularExpressions;

namespace Pliant.Ebnf
{
    public class EbnfVisitor : TreeNodeVisitorBase
    {
        public EbnfDefinition Definition { get; private set; }
        
        public override void Visit(IInternalTreeNode node)
        {
            if (EbnfGrammar.Definition == node.Symbol.Value)
                Definition = VisitDefinitionNode(node);            
        }

        private EbnfDefinition VisitDefinitionNode(IInternalTreeNode node)
        {
            EbnfBlock block = null;
            EbnfDefinition definition = null;
            
            foreach (var child in node.Children)
            {
                switch (child.NodeType)
                {
                    case TreeNodeType.Internal:
                        var internalNode = child as IInternalTreeNode;
                        var symbolValue = internalNode.Symbol.Value;

                        if (EbnfGrammar.Block == symbolValue)
                            block = VisitBlockNode(internalNode);
                        else if (EbnfGrammar.Definition == symbolValue)
                            definition = VisitDefinitionNode(internalNode);
                        break;
                }
            }

            if (definition != null)
                return new EbnfDefinitionRepetition(block, definition);

            return new EbnfDefinition(block);
        }

        private EbnfBlock VisitBlockNode(IInternalTreeNode node)
        {
            foreach (var child in node.Children)
            {
                switch (child.NodeType)
                {
                    case TreeNodeType.Internal:
                        var internalNode = child as IInternalTreeNode;
                        var symbolValue = internalNode.Symbol.Value;

                        if (EbnfGrammar.Rule == symbolValue)
                            return VisitRuleNode(internalNode);
                        if (EbnfGrammar.Setting == symbolValue)
                            return VisitSettingNode(internalNode);
                        if (EbnfGrammar.LexerRule == symbolValue)
                            return VisitLexerRuleNode(internalNode);
                        break;                      
                }
            }
            return null;
        }

        private EbnfBlockRule VisitRuleNode(IInternalTreeNode node)
        {
            EbnfQualifiedIdentifier qualifiedIdentifier = null;
            EbnfExpression expression = null;

            foreach(var child in node.Children)
                switch (child.NodeType)
                {
                    case TreeNodeType.Internal:
                        var internalNode = child as IInternalTreeNode;
                        var symbolValue = internalNode.Symbol.Value;

                        if (EbnfGrammar.QualifiedIdentifier == symbolValue)
                            qualifiedIdentifier = VisitQualifiedIdentifierNode(internalNode);
                        else if (EbnfGrammar.Expression == symbolValue)
                            expression = VisitExpressionNode(internalNode);
                        break;

                    case TreeNodeType.Token:
                        break;
                }

            return new EbnfBlockRule(
                new EbnfRule(qualifiedIdentifier, expression));
        }

        private EbnfQualifiedIdentifier VisitQualifiedIdentifierNode(IInternalTreeNode node)
        {
            EbnfQualifiedIdentifier repetitionIdentifier = null;
            string identifier = null;
            foreach(var child in node.Children)
                switch (child.NodeType)
                {
                    case TreeNodeType.Internal:
                        var internalNode = child as IInternalTreeNode;
                        var symbolValue = internalNode.Symbol.Value;
                        if (EbnfGrammar.QualifiedIdentifier == symbolValue)
                            repetitionIdentifier = VisitQualifiedIdentifierNode(internalNode);
                        break;

                    case TreeNodeType.Token:
                        var tokenNode = child as ITokenTreeNode;
                        var token = tokenNode.Token;
                        if (token.TokenType.Id == "identifier")
                            identifier = token.Value;
                        break;
                }

            if (repetitionIdentifier == null)
                return new EbnfQualifiedIdentifier(identifier);

            return new EbnfQualifiedIdentifierRepetition(identifier, repetitionIdentifier);         
        }

        private EbnfExpression VisitExpressionNode(IInternalTreeNode node)
        {
            EbnfTerm term = null;
            EbnfExpression expression = null;

            foreach(var child in node.Children)
                switch (child.NodeType)
                {
                    case TreeNodeType.Internal:
                        var internalNode = child as IInternalTreeNode;
                        var symbolValue = internalNode.Symbol.Value;
                        if (EbnfGrammar.Term == symbolValue)
                            term = VisitTermNode(internalNode);
                        else if (EbnfGrammar.Expression == symbolValue)
                            expression = VisitExpressionNode(internalNode);
                        break;

                    case TreeNodeType.Token:
                        break;
                }

            if (expression == null)
                return new EbnfExpression(term);
            return new EbnfExpressionAlteration(term, expression);
        }

        private EbnfTerm VisitTermNode(IInternalTreeNode node)
        {
            EbnfFactor factor = null;
            EbnfTerm term = null;

            foreach(var child in node.Children)
                switch (child.NodeType)
                {
                    case TreeNodeType.Internal:
                        var internalNode = child as IInternalTreeNode;
                        var symbolValue = internalNode.Symbol.Value;
                        if (EbnfGrammar.Factor == symbolValue)
                            factor = VisitFactorNode(internalNode);
                        else if (EbnfGrammar.Term == symbolValue)
                            term = VisitTermNode(internalNode);                                
                        break;

                    case TreeNodeType.Token:
                        break;
                }

            if (term == null)
                return new EbnfTerm(factor);
            return new EbnfTermRepetition(factor, term);
        }

        private EbnfFactor VisitFactorNode(IInternalTreeNode node)
        {
            foreach (var child in node.Children)
                switch (child.NodeType)
                {
                    case TreeNodeType.Internal:
                        var internalNode = child as IInternalTreeNode;
                        var symbolValue = internalNode.Symbol.Value;

                        if (EbnfGrammar.QualifiedIdentifier == symbolValue)
                            return new EbnfFactorIdentifier(
                                VisitQualifiedIdentifierNode(internalNode));

                        if (EbnfGrammar.Literal == symbolValue)
                            return new EbnfFactorLiteral(
                                VisitLiteralNode(internalNode));

                        if (EbnfGrammar.Repetition == symbolValue)
                            return VisitRepetitionNode(internalNode);

                        if (EbnfGrammar.Optional == symbolValue)
                            return VisitOptionalNode(internalNode);

                        if (EbnfGrammar.Grouping == symbolValue)
                            return VisitGroupingNode(internalNode);

                        if (RegexGrammar.Regex == symbolValue)
                        {
                            var regexVisitor = new RegexVisitor();
                            internalNode.Accept(regexVisitor);
                            return new EbnfFactorRegex(regexVisitor.Regex);
                        }
                        break;

                    case TreeNodeType.Token:
                        break;
                }
            return null;
        }

        private EbnfFactorRepetition VisitRepetitionNode(IInternalTreeNode node)
        {
            foreach (var child in node.Children)
                switch (child.NodeType)
                {
                    case TreeNodeType.Internal:
                        var internalNode = child as IInternalTreeNode;
                        var symbolValue = internalNode.Symbol.Value;
                        if (EbnfGrammar.Expression == symbolValue)
                        {
                            return new EbnfFactorRepetition(VisitExpressionNode(internalNode));
                        }
                        break;
                }
            return null;
        }

        private EbnfFactorOptional VisitOptionalNode(IInternalTreeNode node)
        {
            foreach (var child in node.Children)
                switch (child.NodeType)
                {
                    case TreeNodeType.Internal:
                        var internalNode = child as IInternalTreeNode;
                        var symbolValue = internalNode.Symbol.Value;
                        if (EbnfGrammar.Expression == symbolValue)
                        {
                            return new EbnfFactorOptional(VisitExpressionNode(internalNode));
                        }
                        break;
                }
            return null;
        }

        private EbnfFactorGrouping VisitGroupingNode(IInternalTreeNode node)
        {
            foreach (var child in node.Children)
                switch (child.NodeType)
                {
                    case TreeNodeType.Internal:
                        var internalNode = child as IInternalTreeNode;
                        var symbolValue = internalNode.Symbol.Value;
                        if (EbnfGrammar.Expression == symbolValue)
                        {
                            return new EbnfFactorGrouping(VisitExpressionNode(internalNode));
                        }
                        break;
                }
            return null;
        }

        private string VisitLiteralNode(IInternalTreeNode node)
        {
            foreach(var child in node.Children)
                switch (child.NodeType)
                {
                    case TreeNodeType.Token:
                        var tokenNode = child as ITokenTreeNode;
                        // TODO: Find a better solution for identifing the lexer rule based on id
                        if (tokenNode.Token.TokenType.Id.Length > 5)
                            return tokenNode.Token.Value;
                        break;

                    case TreeNodeType.Internal:
                        break;
                }
            return null;
        }

        private EbnfBlockSetting VisitSettingNode(IInternalTreeNode node)
        {
            throw new NotImplementedException();
        }
        private EbnfBlockLexerRule VisitLexerRuleNode(IInternalTreeNode node)
        {
            throw new NotImplementedException();
        }

    }
}