using Pliant.Tree;
using Pliant.Grammars;
using System;
using System.Collections.Generic;
using Pliant.RegularExpressions;

namespace Pliant.Ebnf
{
    public class EbnfVisitor : TreeNodeVisitorBase
    {
        public IGrammar Grammar { get { return _grammar; } }

        private Grammar _grammar;

        private const string GrammarSymbol = "Grammar";
        private const string BlockSymbol = "Block";
        private const string RuleSymbol = "Rule";
        private const string QualifiedIdentifierSymbol = "QualifiedIdentifier";
        private const string ExpressionSymbol = "Expression";
        private const string TermSymbol = "Term";
        private const string FactorSymbol = "Factor";
        private const string LiteralSymbol = "Literal";
        private const string RegexSymbol = "Regex";
        private const string RepetitionSymbol = "Repetition";
        private const string OptionalSymbol = "Optional";
        private const string GroupingSymbol = "Grouping";

        public EbnfVisitor()
            : base()
        {
        }

        public override void Visit(IInternalTreeNode node)
        {
            switch (node.Symbol.Value)
            {
                case GrammarSymbol:
                    _grammar = new Grammar();
                    base.Visit(node);
                    if (_grammar.Start == null)
                        _grammar.Start = _grammar.Productions[0].LeftHandSide;                    
                    break;

                case BlockSymbol:
                    base.Visit(node);
                    break;

                case RuleSymbol:
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
                    case QualifiedIdentifierSymbol:
                        productionName = GetNameFromQualifiedIdentifierNode(internalChild);
                        break;

                    case ExpressionSymbol:
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
                    case TermSymbol:
                        var termListOfSymbolList = GetListOfSymbolListFromTermNode(internalChild);
                        // TODO: Check if this is this right. May need to add to each item in the range
                        listOfSymbolLists.AddRange(termListOfSymbolList);
                        break;

                    case ExpressionSymbol:
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
                    case FactorSymbol:
                        var factorListOfSymbolList = GetListOfSymbolListFromFactorNode(internalChild);
                        listOfSymbolList.AddRange(factorListOfSymbolList);
                        break;

                    case TermSymbol:
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
                    case QualifiedIdentifierSymbol:
                        break;

                    case LiteralSymbol:
                        var stringLiteral = GetLexerRuleFromLiteralNode(internalChild);
                        symbolList.Add(stringLiteral);
                        break;

                    case RegexSymbol:
                        var lexerRule = GetLexerRuleFromRegexNode(internalChild);
                        symbolList.Add(lexerRule);
                        break;

                    case RepetitionSymbol:
                        throw new NotSupportedException("Repetition is not currently supprted because implementation would require adding new rules to the grammar.");

                    case OptionalSymbol:
                        throw new NotSupportedException("Optional is not currently supported due to complexity in implementation.");

                    case GroupingSymbol:
                        throw new NotSupportedException("Grouping is not currently supported due to complexity in implementation.");
                }
            }
            listOfSymbolList.Add(symbolList);
            return listOfSymbolList;
        }

        private static ILexerRule GetLexerRuleFromRegexNode(IInternalTreeNode internalChild)
        {
            var regexVisitor = new RegexVisitor();
            internalChild.Accept(regexVisitor);
            var regex = regexVisitor.Regex;
            var regexCompiler = new RegexCompiler();
            var lexerRule = regexCompiler.Compile(regex);
            return lexerRule;
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
