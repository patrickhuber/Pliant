using System;
using Pliant.Tree;
using Pliant.Languages.Regex;
using Pliant.LexerRules;
using Pliant.Captures;

namespace Pliant.Languages.Pdl
{
    public class PdlVisitor : TreeNodeVisitorBase
    {
        public PdlDefinition Definition { get; private set; }
        
        public override void Visit(IInternalTreeNode node)
        {
            if (PdlGrammar.Definition == node.Symbol.Value)
                Definition = VisitDefinitionNode(node);            
        }

        private PdlDefinition VisitDefinitionNode(IInternalTreeNode node)
        {
            PdlBlock block = null;
            PdlDefinition definition = null;
            
            for (int c = 0; c< node.Children.Count; c++)
            {
                var child = node.Children[c];
                switch (child.NodeType)
                {
                    case TreeNodeType.Internal:
                        var internalNode = child as IInternalTreeNode;
                        var symbolValue = internalNode.Symbol.Value;

                        if (PdlGrammar.Block == symbolValue)
                            block = VisitBlockNode(internalNode);
                        else if (PdlGrammar.Definition == symbolValue)
                            definition = VisitDefinitionNode(internalNode);
                        break;
                }
            }

            if (definition != null)
                return new PdlDefinitionConcatenation(block, definition);

            return new PdlDefinition(block);
        }

        private PdlBlock VisitBlockNode(IInternalTreeNode node)
        {
            for (int c = 0; c < node.Children.Count; c++)
            {
                var child = node.Children[c];
                switch (child.NodeType)
                {
                    case TreeNodeType.Internal:
                        var internalNode = child as IInternalTreeNode;
                        var symbolValue = internalNode.Symbol.Value;

                        if (PdlGrammar.Rule == symbolValue)
                            return VisitRuleNode(internalNode);
                        if (PdlGrammar.Setting == symbolValue)
                            return VisitSettingNode(internalNode);
                        if (PdlGrammar.LexerRule == symbolValue)
                            return VisitLexerRuleNode(internalNode);
                        break;                      
                }
            }
            throw UnreachableCodeException();
        }

        private PdlBlockRule VisitRuleNode(IInternalTreeNode node)
        {
            PdlQualifiedIdentifier qualifiedIdentifier = null;
            PdlExpression expression = null;

            for (int c = 0; c < node.Children.Count; c++)
            {
                var child = node.Children[c];
                switch (child.NodeType)
                {
                    case TreeNodeType.Internal:
                        var internalNode = child as IInternalTreeNode;
                        var symbolValue = internalNode.Symbol.Value;

                        if (PdlGrammar.QualifiedIdentifier == symbolValue)
                            qualifiedIdentifier = VisitQualifiedIdentifierNode(internalNode);
                        else if (PdlGrammar.Expression == symbolValue)
                            expression = VisitExpressionNode(internalNode);
                        break;

                    case TreeNodeType.Token:
                        break;
                }
            }
            return new PdlBlockRule(
                new PdlRule(qualifiedIdentifier, expression));
        }

        private PdlQualifiedIdentifier VisitQualifiedIdentifierNode(IInternalTreeNode node)
        {
            PdlQualifiedIdentifier repetitionIdentifier = null;
            ICapture<char> identifier = null;
            for (int c = 0; c < node.Children.Count; c++)
            {
                var child = node.Children[c];
                switch (child.NodeType)
                {
                    case TreeNodeType.Internal:
                        var internalNode = child as IInternalTreeNode;
                        var symbolValue = internalNode.Symbol.Value;
                        if (PdlGrammar.QualifiedIdentifier == symbolValue)
                            repetitionIdentifier = VisitQualifiedIdentifierNode(internalNode);
                        break;

                    case TreeNodeType.Token:
                        var tokenNode = child as ITokenTreeNode;
                        var token = tokenNode.Token;
                        if (token.TokenType.Equals(PdlGrammar.TokenTypes.Identifier))
                            identifier = token.Capture;
                        break;
                }
            }
            if (repetitionIdentifier is null)
                return new PdlQualifiedIdentifier(identifier);

            return new PdlQualifiedIdentifierConcatenation(identifier, repetitionIdentifier);         
        }

        private PdlExpression VisitExpressionNode(IInternalTreeNode node)
        {
            PdlTerm term = null;
            PdlExpression expression = null;

            for (int c = 0; c < node.Children.Count; c++)
            {
                var child = node.Children[c];
                switch (child.NodeType)
                {
                    case TreeNodeType.Internal:
                        var internalNode = child as IInternalTreeNode;
                        var symbolValue = internalNode.Symbol.Value;
                        if (PdlGrammar.Term == symbolValue)
                            term = VisitTermNode(internalNode);
                        else if (PdlGrammar.Expression == symbolValue)
                            expression = VisitExpressionNode(internalNode);
                        break;

                    case TreeNodeType.Token:
                        break;
                }
            }
            if (expression is null)
                return new PdlExpression(term);
            return new PdlExpressionAlteration(term, expression);
        }

        private PdlTerm VisitTermNode(IInternalTreeNode node)
        {
            PdlFactor factor = null;
            PdlTerm term = null;

            for (int c = 0; c < node.Children.Count; c++)
            {
                var child = node.Children[c];
                switch (child.NodeType)
                {
                    case TreeNodeType.Internal:
                        var internalNode = child as IInternalTreeNode;
                        var symbolValue = internalNode.Symbol.Value;
                        if (PdlGrammar.Factor == symbolValue)
                            factor = VisitFactorNode(internalNode);
                        else if (PdlGrammar.Term == symbolValue)
                            term = VisitTermNode(internalNode);
                        break;

                    case TreeNodeType.Token:
                        break;
                }
            }
            if (term is null)
                return new PdlTerm(factor);
            return new PdlTermConcatenation(factor, term);
        }

        private PdlFactor VisitFactorNode(IInternalTreeNode node)
        {
            for (int c = 0; c < node.Children.Count; c++)
            {
                var child = node.Children[c];
                switch (child.NodeType)
                {
                    case TreeNodeType.Internal:
                        var internalNode = child as IInternalTreeNode;
                        var symbolValue = internalNode.Symbol.Value;

                        if (PdlGrammar.QualifiedIdentifier == symbolValue)
                            return new PdlFactorIdentifier(
                                VisitQualifiedIdentifierNode(internalNode));

                        if (PdlGrammar.Literal == symbolValue)
                            return new PdlFactorLiteral(
                                VisitLiteralNode(internalNode));

                        if (PdlGrammar.Repetition == symbolValue)
                            return VisitRepetitionNode(internalNode);

                        if (PdlGrammar.Optional == symbolValue)
                            return VisitOptionalNode(internalNode);

                        if (PdlGrammar.Grouping == symbolValue)
                            return VisitGroupingNode(internalNode);

                        if (RegexGrammar.Regex == symbolValue)
                        {
                            var regexVisitor = new RegexVisitor();
                            internalNode.Accept(regexVisitor);
                            return new PdlFactorRegex(regexVisitor.Regex);
                        }
                        break;

                    case TreeNodeType.Token:
                        break;
                }
            }
            throw UnreachableCodeException();
        }

        private PdlFactorRepetition VisitRepetitionNode(IInternalTreeNode node)
        {
            for (int c = 0; c < node.Children.Count; c++)
            {
                var child = node.Children[c];
                switch (child.NodeType)
                {
                    case TreeNodeType.Internal:
                        var internalNode = child as IInternalTreeNode;
                        var symbolValue = internalNode.Symbol.Value;
                        if (PdlGrammar.Expression == symbolValue)
                        {
                            return new PdlFactorRepetition(VisitExpressionNode(internalNode));
                        }
                        break;
                }
            }
            throw UnreachableCodeException();
        }

        private PdlFactorOptional VisitOptionalNode(IInternalTreeNode node)
        {
            for (int c = 0; c < node.Children.Count; c++)
            {
                var child = node.Children[c];
                switch (child.NodeType)
                {
                    case TreeNodeType.Internal:
                        var internalNode = child as IInternalTreeNode;
                        var symbolValue = internalNode.Symbol.Value;
                        if (PdlGrammar.Expression == symbolValue)
                        {
                            return new PdlFactorOptional(VisitExpressionNode(internalNode));
                        }
                        break;
                }
            }
            throw UnreachableCodeException();
        }

        private PdlFactorGrouping VisitGroupingNode(IInternalTreeNode node)
        {
            for (int c = 0; c < node.Children.Count; c++)
            {
                var child = node.Children[c];
                switch (child.NodeType)
                {
                    case TreeNodeType.Internal:
                        var internalNode = child as IInternalTreeNode;
                        var symbolValue = internalNode.Symbol.Value;
                        if (PdlGrammar.Expression == symbolValue)
                        {
                            return new PdlFactorGrouping(VisitExpressionNode(internalNode));
                        }
                        break;
                }
            }
            throw UnreachableCodeException();
        }

        private static ICapture<char> VisitLiteralNode(IInternalTreeNode node)
        {
            for (int c = 0; c < node.Children.Count; c++)
            {
                var child = node.Children[c];
                switch (child.NodeType)
                {
                    case TreeNodeType.Token:
                        var tokenNode = child as ITokenTreeNode;
                        var token = tokenNode.Token;
                        var tokenType = token.TokenType;

                        // if token type is string token type remove surrounding quotes
                        if (tokenType.Equals(SingleQuoteStringLexerRule.TokenTypeDescriptor)
                            || tokenType.Equals(DoubleQuoteStringLexerRule.TokenTypeDescriptor))
                            return token.Capture.Slice(1, token.Capture.Count -2);

                        // TODO: Find a better solution for identifing the lexer rule based on id
                        if (tokenNode.Token.TokenType.Id.Length > 5)
                            return token.Capture;

                        break;

                    case TreeNodeType.Internal:
                        break;
                }
            }
            throw UnreachableCodeException();
        }

        private PdlBlockSetting VisitSettingNode(IInternalTreeNode node)
        {
            PdlSettingIdentifier settingIdentifier = null;
            PdlQualifiedIdentifier qualifiedIdentifier = null;

            for (int c = 0; c < node.Children.Count; c++)
            {
                var child = node.Children[c];
                switch (child.NodeType)
                {
                    case TreeNodeType.Token:
                        var tokenNode = child as ITokenTreeNode;
                        var token = tokenNode.Token;
                        if (token.TokenType.Equals(PdlGrammar.TokenTypes.SettingIdentifier))
                            settingIdentifier = new PdlSettingIdentifier(token.Capture);
                        break;

                    case TreeNodeType.Internal:
                        var internalNode = child as IInternalTreeNode;
                        var symbolValue = internalNode.Symbol.Value;
                        if (PdlGrammar.QualifiedIdentifier == symbolValue)
                            qualifiedIdentifier = VisitQualifiedIdentifierNode(internalNode);
                        break;
                }
            }
            return new PdlBlockSetting(
                new PdlSetting(settingIdentifier, qualifiedIdentifier));
        }

        private PdlBlockLexerRule VisitLexerRuleNode(IInternalTreeNode node)
        {
            PdlQualifiedIdentifier qualifiedIdentifier = null;
            PdlLexerRuleExpression expression = null;

            for (int c = 0; c < node.Children.Count; c++)
            {
                var child = node.Children[c];
                if (child.NodeType != TreeNodeType.Internal)
                    continue;
                var internalNode = child as IInternalTreeNode;
                var symbolValue = internalNode.Symbol.Value;
                if (PdlGrammar.QualifiedIdentifier == symbolValue)
                    qualifiedIdentifier = VisitQualifiedIdentifierNode(internalNode);
                else if (PdlGrammar.LexerRuleExpression == symbolValue)
                    expression = VisitLexerRuleExpressionNode(internalNode);
                
            }
            return new PdlBlockLexerRule(
                new PdlLexerRule(qualifiedIdentifier, expression));
        }

        private PdlLexerRuleExpression VisitLexerRuleExpressionNode(IInternalTreeNode node)
        {
            PdlLexerRuleTerm term = null;
            PdlLexerRuleExpression expression = null;

            for (int c = 0; c < node.Children.Count; c++)
            {
                var child = node.Children[c];
                if (child.NodeType != TreeNodeType.Internal)
                    continue;
                var internalNode = child as IInternalTreeNode;
                var symbolValue = internalNode.Symbol.Value;
                if (PdlGrammar.LexerRuleTerm == symbolValue)
                    term = VisitLexerRuleTermNode(internalNode);
                if (PdlGrammar.LexerRuleExpression == symbolValue)
                    expression = VisitLexerRuleExpressionNode(internalNode);
            }

            if (expression is null)
                return new PdlLexerRuleExpression(term);
            return new PdlLexerRuleExpressionAlteration(term, expression);
        }

        private PdlLexerRuleTerm VisitLexerRuleTermNode(IInternalTreeNode node)
        {
            PdlLexerRuleFactor factor = null;
            PdlLexerRuleTerm term = null;

            for (int c = 0; c < node.Children.Count; c++)
            {
                var child = node.Children[c];
                if (child.NodeType != TreeNodeType.Internal)
                    continue;
                var internalNode = child as IInternalTreeNode;
                var symbolValue = internalNode.Symbol.Value;
                if (PdlGrammar.LexerRuleFactor == symbolValue)
                    factor = VisitLexerRuleFactorNode(internalNode);
                if (PdlGrammar.LexerRuleTerm == symbolValue)
                    term = VisitLexerRuleTermNode(internalNode);
            }

            if (term is null)
                return new PdlLexerRuleTerm(factor);

            return new PdlLexerRuleTermConcatenation(factor, term);
        }

        private static PdlLexerRuleFactor VisitLexerRuleFactorNode(IInternalTreeNode node)
        {
            for (int c = 0; c < node.Children.Count; c++)
            {
                var child = node.Children[c];
                if (child.NodeType != TreeNodeType.Internal)
                    continue;
                var internalNode = child as IInternalTreeNode;
                var symbolValue = internalNode.Symbol.Value;

                if (PdlGrammar.Literal == symbolValue)                
                    return new PdlLexerRuleFactorLiteral(
                        VisitLiteralNode(internalNode));
                
                if (RegexGrammar.Regex == symbolValue)
                {
                    var regexVisitor = new RegexVisitor();
                    internalNode.Accept(regexVisitor);
                    return new PdlLexerRuleFactorRegex(regexVisitor.Regex);
                }
            }
            throw UnreachableCodeException();
        }

        private static Exception UnreachableCodeException()
        {
            return new InvalidOperationException("Unreachable Code Detected");
        }
    }
}