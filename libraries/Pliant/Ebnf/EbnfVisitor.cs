using System;
using Pliant.Tree;
using Pliant.RegularExpressions;
using Pliant.LexerRules;

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
            
            for (int c = 0; c< node.Children.Count; c++)
            {
                var child = node.Children[c];
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
                return new EbnfDefinitionConcatenation(block, definition);

            return new EbnfDefinition(block);
        }

        private EbnfBlock VisitBlockNode(IInternalTreeNode node)
        {
            for (int c = 0; c < node.Children.Count; c++)
            {
                var child = node.Children[c];
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
            throw UnreachableCodeException();
        }

        private EbnfBlockRule VisitRuleNode(IInternalTreeNode node)
        {
            EbnfQualifiedIdentifier qualifiedIdentifier = null;
            EbnfExpression expression = null;

            for (int c = 0; c < node.Children.Count; c++)
            {
                var child = node.Children[c];
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
            }
            return new EbnfBlockRule(
                new EbnfRule(qualifiedIdentifier, expression));
        }

        private EbnfQualifiedIdentifier VisitQualifiedIdentifierNode(IInternalTreeNode node)
        {
            EbnfQualifiedIdentifier repetitionIdentifier = null;
            string identifier = null;
            for (int c = 0; c < node.Children.Count; c++)
            {
                var child = node.Children[c];
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
                        if (token.TokenType.Equals(EbnfGrammar.TokenTypes.Identifier))
                            identifier = token.Value;
                        break;
                }
            }
            if (repetitionIdentifier == null)
                return new EbnfQualifiedIdentifier(identifier);

            return new EbnfQualifiedIdentifierConcatenation(identifier, repetitionIdentifier);         
        }

        private EbnfExpression VisitExpressionNode(IInternalTreeNode node)
        {
            EbnfTerm term = null;
            EbnfExpression expression = null;

            for (int c = 0; c < node.Children.Count; c++)
            {
                var child = node.Children[c];
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
            }
            if (expression == null)
                return new EbnfExpression(term);
            return new EbnfExpressionAlteration(term, expression);
        }

        private EbnfTerm VisitTermNode(IInternalTreeNode node)
        {
            EbnfFactor factor = null;
            EbnfTerm term = null;

            for (int c = 0; c < node.Children.Count; c++)
            {
                var child = node.Children[c];
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
            }
            if (term == null)
                return new EbnfTerm(factor);
            return new EbnfTermConcatenation(factor, term);
        }

        private EbnfFactor VisitFactorNode(IInternalTreeNode node)
        {
            for (int c = 0; c < node.Children.Count; c++)
            {
                var child = node.Children[c];
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
            }
            throw UnreachableCodeException();
        }

        private EbnfFactorRepetition VisitRepetitionNode(IInternalTreeNode node)
        {
            for (int c = 0; c < node.Children.Count; c++)
            {
                var child = node.Children[c];
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
            }
            throw UnreachableCodeException();
        }

        private EbnfFactorOptional VisitOptionalNode(IInternalTreeNode node)
        {
            for (int c = 0; c < node.Children.Count; c++)
            {
                var child = node.Children[c];
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
            }
            throw UnreachableCodeException();
        }

        private EbnfFactorGrouping VisitGroupingNode(IInternalTreeNode node)
        {
            for (int c = 0; c < node.Children.Count; c++)
            {
                var child = node.Children[c];
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
            }
            throw UnreachableCodeException();
        }

        private static string VisitLiteralNode(IInternalTreeNode node)
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
                            return token.Value.Substring(1, token.Value.Length - 2);

                        // TODO: Find a better solution for identifing the lexer rule based on id
                        if (tokenNode.Token.TokenType.Id.Length > 5)
                            return token.Value;

                        break;

                    case TreeNodeType.Internal:
                        break;
                }
            }
            throw UnreachableCodeException();
        }

        private EbnfBlockSetting VisitSettingNode(IInternalTreeNode node)
        {
            EbnfSettingIdentifier settingIdentifier = null;
            EbnfQualifiedIdentifier qualifiedIdentifier = null;

            for (int c = 0; c < node.Children.Count; c++)
            {
                var child = node.Children[c];
                switch (child.NodeType)
                {
                    case TreeNodeType.Token:
                        var tokenNode = child as ITokenTreeNode;
                        var token = tokenNode.Token;
                        if (token.TokenType.Equals(EbnfGrammar.TokenTypes.SettingIdentifier))
                            settingIdentifier = new EbnfSettingIdentifier(token.Value);
                        break;

                    case TreeNodeType.Internal:
                        var internalNode = child as IInternalTreeNode;
                        var symbolValue = internalNode.Symbol.Value;
                        if (EbnfGrammar.QualifiedIdentifier == symbolValue)
                            qualifiedIdentifier = VisitQualifiedIdentifierNode(internalNode);
                        break;
                }
            }
            return new EbnfBlockSetting(
                new EbnfSetting(settingIdentifier, qualifiedIdentifier));
        }

        private EbnfBlockLexerRule VisitLexerRuleNode(IInternalTreeNode node)
        {
            EbnfQualifiedIdentifier qualifiedIdentifier = null;
            EbnfLexerRuleExpression expression = null;

            for (int c = 0; c < node.Children.Count; c++)
            {
                var child = node.Children[c];
                if (child.NodeType != TreeNodeType.Internal)
                    continue;
                var internalNode = child as IInternalTreeNode;
                var symbolValue = internalNode.Symbol.Value;
                if (EbnfGrammar.QualifiedIdentifier == symbolValue)
                    qualifiedIdentifier = VisitQualifiedIdentifierNode(internalNode);
                else if (EbnfGrammar.LexerRuleExpression == symbolValue)
                    expression = VisitLexerRuleExpressionNode(internalNode);
                
            }
            return new EbnfBlockLexerRule(
                new EbnfLexerRule(qualifiedIdentifier, expression));
        }

        private EbnfLexerRuleExpression VisitLexerRuleExpressionNode(IInternalTreeNode node)
        {
            EbnfLexerRuleTerm term = null;
            EbnfLexerRuleExpression expression = null;

            for (int c = 0; c < node.Children.Count; c++)
            {
                var child = node.Children[c];
                if (child.NodeType != TreeNodeType.Internal)
                    continue;
                var internalNode = child as IInternalTreeNode;
                var symbolValue = internalNode.Symbol.Value;
                if (EbnfGrammar.LexerRuleTerm == symbolValue)
                    term = VisitLexerRuleTermNode(internalNode);
                if (EbnfGrammar.LexerRuleExpression == symbolValue)
                    expression = VisitLexerRuleExpressionNode(internalNode);
            }

            if (expression == null)
                return new EbnfLexerRuleExpression(term);
            return new EbnfLexerRuleExpressionAlteration(term, expression);
        }

        private EbnfLexerRuleTerm VisitLexerRuleTermNode(IInternalTreeNode node)
        {
            EbnfLexerRuleFactor factor = null;
            EbnfLexerRuleTerm term = null;

            for (int c = 0; c < node.Children.Count; c++)
            {
                var child = node.Children[c];
                if (child.NodeType != TreeNodeType.Internal)
                    continue;
                var internalNode = child as IInternalTreeNode;
                var symbolValue = internalNode.Symbol.Value;
                if (EbnfGrammar.LexerRuleFactor == symbolValue)
                    factor = VisitLexerRuleFactorNode(internalNode);
                if (EbnfGrammar.LexerRuleTerm == symbolValue)
                    term = VisitLexerRuleTermNode(internalNode);
            }

            if (term == null)
                return new EbnfLexerRuleTerm(factor);

            return new EbnfLexerRuleTermConcatenation(factor, term);
        }

        private static EbnfLexerRuleFactor VisitLexerRuleFactorNode(IInternalTreeNode node)
        {
            for (int c = 0; c < node.Children.Count; c++)
            {
                var child = node.Children[c];
                if (child.NodeType != TreeNodeType.Internal)
                    continue;
                var internalNode = child as IInternalTreeNode;
                var symbolValue = internalNode.Symbol.Value;

                if (EbnfGrammar.Literal == symbolValue)                
                    return new EbnfLexerRuleFactorLiteral(
                        VisitLiteralNode(internalNode));
                
                if (RegexGrammar.Regex == symbolValue)
                {
                    var regexVisitor = new RegexVisitor();
                    internalNode.Accept(regexVisitor);
                    return new EbnfLexerRuleFactorRegex(regexVisitor.Regex);
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