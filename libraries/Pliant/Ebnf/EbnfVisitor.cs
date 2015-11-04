using Pliant.Tree;
using Pliant.Grammars;
using Pliant.Builders;
using System;
using System.Collections.Generic;

namespace Pliant.Ebnf
{
    public class EbnfVisitor : TreeNodeVisitorBase
    {
        public IGrammar Grammar { get { return _grammarbuilder.ToGrammar(); } }

        private GrammarBuilder _grammarbuilder;
        private Grammar _grammar;

        public EbnfVisitor()
            : base()
        {
        }

        public override void Visit(IInternalTreeNode node)
        {
            switch (node.Symbol.Value)
            {
                case "Grammar":
                    _grammarbuilder = new GrammarBuilder();
                    _grammar = new Grammar();
                    base.Visit(node);
                    break;

                case "Block":
                    base.Visit(node);
                    break;

                case "Rule":
                    var builder = GetProductionBuilderFromRuleNode(node);
                    _grammarbuilder.AddProduction(builder);
                    break;
            }
        }

        private ProductionBuilder GetProductionBuilderFromRuleNode(IInternalTreeNode node)
        {
            ProductionBuilder productionBuilder = null;
            foreach (var child in node.Children)
            {
                if (child.NodeType != TreeNodeType.Internal)
                    continue;
                var internalChild = child as IInternalTreeNode;
                switch (internalChild.Symbol.Value)
                {
                    case "QualifiedIdentifier":
                        var name = GetNameFromQualifiedIdentifierNode(internalChild);
                        productionBuilder = new ProductionBuilder(name);
                        break;

                    case "Expression":
                        var baseBuilderList = new BaseBuilderList();
                        baseBuilderList = VisitExpressionNode(baseBuilderList, internalChild);
                        var ruleBuilder = new RuleBuilder();
                        ruleBuilder.Data.Add(baseBuilderList);
                        productionBuilder.Definition = ruleBuilder;
                        break;

                    default:
                        break;
                }
            }
            return productionBuilder; 
        }
        
        private BaseBuilderList VisitExpressionNode(
            BaseBuilderList builderList, IInternalTreeNode expressionNode)
        {            
            foreach (var child in expressionNode.Children)
            {
                if (child.NodeType != TreeNodeType.Internal)
                    continue;

                var internalChild = child as IInternalTreeNode;
                switch (internalChild.Symbol.Value)
                {
                    case "Term":
                        builderList = VisitTermNode(builderList, internalChild);
                        break;

                    case "Expression":
                        builderList = VisitExpressionNode(builderList.CreateAlterations(), internalChild);
                        break;
                }
            }
            return builderList;
        }

        private BaseBuilderList VisitTermNode(
            BaseBuilderList builderList, IInternalTreeNode termNode)
        {
            foreach (var child in termNode.Children)
            {
                if (child.NodeType != TreeNodeType.Internal)
                    continue;

                var internalChild = child as IInternalTreeNode;
                switch (internalChild.Symbol.Value)
                {
                    case "Factor":
                        builderList = VisitFactorNode(builderList, internalChild);
                        break;

                    case "Term":
                        builderList = VisitTermNode(builderList, internalChild);
                        break;
                }
            }
            return builderList;
        }

        private BaseBuilderList VisitFactorNode(
            BaseBuilderList builderList, IInternalTreeNode factorNode)
        {
            foreach (var child in factorNode.Children)
            {
                if (child.NodeType != TreeNodeType.Internal)
                    continue;

                var internalChild = child as IInternalTreeNode;
                switch (internalChild.Symbol.Value)
                {
                    case "QualifiedIdentfiier":
                        break;

                    case "Literal":
                        SymbolBuilder stringLiteral = GetLexerRuleFromLiteralNode(internalChild);
                        builderList.Add(stringLiteral);
                        break;

                    case "Regex":
                        throw new NotSupportedException("Regex requires implementing a regex complier");
                        
                    case "Repetition":                           
                        throw new NotSupportedException("Repetition is not currently supprted because implementation would require adding new rules to the grammar.");
                        
                    case "Optional":
                        throw new NotSupportedException("Optional is not currently supported due to complexity in implementation.");

                    case "Grouping":
                        throw new NotSupportedException("Grouping is not currently supported due to complexity in implementation.");
                }
            }
            return builderList;
        }

        private BaseLexerRule GetLexerRuleFromLiteralNode(IInternalTreeNode node)
        {
            foreach (var child in node.Children)
            {
                if (child.NodeType != TreeNodeType.Token)
                    continue;
                var tokenNode = child as ITokenTreeNode;                
                if (tokenNode.Token.TokenType.Id != "'" &&
                    tokenNode.Token.TokenType.Id != "\"")
                {
                    var value = tokenNode.Token.Value;
                    return new StringLiteralLexerRule(value);
                }
            }
            throw new Exception("invalid string literal.");   
        }

        private string GetNameFromQualifiedIdentifierNode(IInternalTreeNode qualfieidIdentifier)
        {
            foreach (var child in qualfieidIdentifier.Children)
            {
                if (child.NodeType == TreeNodeType.Token)
                {
                    var tokenNode = child as ITokenTreeNode;
                    return tokenNode.Token.Value;
                }
                break;
            }
            return null;
        }

        public override void Visit(ITokenTreeNode node)
        {
            base.Visit(node);
        }
    }
}
