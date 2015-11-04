using Pliant.Tree;
using Pliant.Grammars;
using Pliant.Builders;
using System;
using System.Collections.Generic;

namespace Pliant.Ebnf
{
    public class EbnfVisitor : TreeNodeVisitorBase
    {
        public IGrammar Grammar { get { return _grammar; } }

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
                    _grammar = new Grammar();
                    base.Visit(node);
                    if (_grammar.Start == null)
                        _grammar.Start = _grammar.Productions[0].LeftHandSide;                    
                    break;

                case "Block":
                    base.Visit(node);
                    break;

                case "Rule":
                    var productions = GetProductionListFromRuleNode(node);
                    foreach (var production in productions)
                        _grammar.AddProduction(production);
                    break;
            }
        }

        private class SymbolList : List<ISymbol>
        {
        }

        private List<Production> GetProductionListFromRuleNode(IInternalTreeNode node)
        {
            var productionList = new List<Production>();
            var productionName = string.Empty;
            foreach (var child in node.Children)
            {
                if (child.NodeType != TreeNodeType.Internal)
                    continue;

                var internalChild = child as IInternalTreeNode;
                switch (internalChild.Symbol.Value)
                {
                    case "QualifiedIdentifier":
                        productionName = GetNameFromQualifiedIdentifierNode(internalChild);
                        break;

                    case "Expression":
                        var listOfSymbolLists = GetListOfSymbolListFromExpressionNode(internalChild);
                        foreach (var symbolList in listOfSymbolLists)
                        {
                            var production = new Production(productionName);
                            foreach (var symbol in symbolList)
                                production.AddSymbol(symbol);
                            productionList.Add(production);
                        } 
                        break;
                }
            }
            return productionList;
        }

        private List<SymbolList> GetListOfSymbolListFromExpressionNode(
            IInternalTreeNode expressionNode)
        {
            var listOfSymbolLists = new List<SymbolList>();
            foreach (var child in expressionNode.Children)
            {
                if (child.NodeType != TreeNodeType.Internal)
                    continue;

                var internalChild = child as IInternalTreeNode;
                switch (internalChild.Symbol.Value)
                {
                    case "Term":
                        var termListOfSymbolList = GetListOfSymbolListFromTermNode(internalChild);
                        // TODO: Check if this is this right. May need to add to each item in the range
                        listOfSymbolLists.AddRange(termListOfSymbolList);
                        break;

                    case "Expression":
                        var expressionListOfSymbolList = GetListOfSymbolListFromExpressionNode(internalChild);
                        listOfSymbolLists.AddRange(expressionListOfSymbolList);
                        break;
                }
            }
            return listOfSymbolLists;
        }

        private List<SymbolList> GetListOfSymbolListFromTermNode(IInternalTreeNode termNode)
        {
            var listOfSymbolList = new List<SymbolList>();
            foreach (var child in termNode.Children)
            {
                if (child.NodeType != TreeNodeType.Internal)
                    continue;

                var internalChild = child as IInternalTreeNode;
                switch (internalChild.Symbol.Value)
                {
                    case "Factor":
                        var factorListOfSymbolList = GetListOfSymbolListFromFactorNode(internalChild);
                        listOfSymbolList.AddRange(factorListOfSymbolList);
                        break;

                    case "Term":
                        var termListOfSymbolList = GetListOfSymbolListFromTermNode(internalChild);
                        foreach (var currentSymbolList in listOfSymbolList)
                        {
                            foreach (var termSymbolList in termListOfSymbolList)
                            {
                                currentSymbolList.AddRange(termSymbolList);
                            }
                        }
                        break;
                }
            }
            return listOfSymbolList;
        }

        private List<SymbolList> GetListOfSymbolListFromFactorNode(IInternalTreeNode factorNode)
        {
            var listOfSymbolList = new List<SymbolList>();
            var symbolList = new SymbolList();
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
                        var stringLiteral = GetLexerRuleFromLiteralNode(internalChild);
                        symbolList.Add(stringLiteral);
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
            listOfSymbolList.Add(symbolList);
            return listOfSymbolList;
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
