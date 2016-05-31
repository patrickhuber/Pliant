using Pliant.Collections;
using Pliant.Grammars;
using Pliant.Tokens;
using System;
using System.Collections.Generic;

namespace Pliant.Builders
{
    public class RuleBuilder : BaseBuilder, IRuleBuilder
    {
        public List<BaseBuilderList> Data { get; private set; }

        public override ISymbol Symbol
        {
            get { throw new NotImplementedException(); }
        }

        public RuleBuilder()
        {
            Data = new List<BaseBuilderList>();
        }

        public RuleBuilder(BaseBuilder baseBuilder)
            : this()
        {
            AddWithAnd(baseBuilder);
        }

        public void AddWithAnd(BaseBuilder symbolBuilder)
        {
            if (Data.Count == 0)
                Data.Add(new BaseBuilderList());
            Data[Data.Count - 1].Add(symbolBuilder);
        }

        public void AddWithOr(BaseBuilder symbolBuilder)
        {
            var list = new BaseBuilderList();
            list.Add(symbolBuilder);
            Data.Add(list);
        }

        public void AddOptional(ISymbol symbol)
        {
            var newData = new List<BaseBuilderList>();
            foreach (var list in Data)
            {
                // clone each window list
                var listWithSymbol = new BaseBuilderList();
                listWithSymbol.AddRange(list);

                // add the symbol to the clone
                listWithSymbol.Add(new SymbolBuilder(symbol));

                // add the listWithSymbol to the aggregate of new data
                newData.Add(listWithSymbol);
            }

            // add the new data to the list of existing data
            Data.AddRange(newData);
        }

        public IRuleBuilder Rule(params object[] symbols)
        {
            var symbolList = new BaseBuilderList();
            foreach (var symbol in symbols)
            {
                if (symbol is char)
                {
                    var terminal = new CharacterTerminal((char)symbol);
                    var lexerRule = new TerminalLexerRule(
                        terminal,
                        new TokenType(terminal.ToString()));
                    symbolList.Add(
                        new SymbolBuilder(lexerRule));
                }
                else if (symbol is ITerminal)
                {
                    var terminal = symbol as ITerminal;
                    var lexerRule = new TerminalLexerRule(
                        terminal,
                        new TokenType(terminal.ToString()));
                    symbolList.Add(
                        new SymbolBuilder(lexerRule));
                }
                else if (symbol is ILexerRule)
                {
                    symbolList.Add(
                        new SymbolBuilder(symbol as ILexerRule));
                }
                else if (symbol is string)
                {
                    var terminal = new StringLiteralLexerRule(symbol as string);
                    symbolList.Add(
                        new SymbolBuilder(terminal));
                }
                else if (symbol == null)
                { }
                else { throw new ArgumentException("unrecognized terminal or nonterminal"); }
            }

            Data.Add(symbolList);
            return this;
        }

        public IRuleBuilder Lambda()
        {
            return Rule();
        }

        public static implicit operator RuleBuilder(ProductionBuilder productionBuilder)
        {
            return new RuleBuilder(productionBuilder);
        }

        public static implicit operator RuleBuilder(string literal)
        {
            return new RuleBuilder(
                new SymbolBuilder(
                    new StringLiteralLexerRule(literal)));
        }

        public static implicit operator RuleBuilder(char literal)
        {
            return new RuleBuilder(
                new SymbolBuilder(
                    new TerminalLexerRule(literal)));
        }

        public static implicit operator RuleBuilder(BaseTerminal terminal)
        {
            return new RuleBuilder(
                new SymbolBuilder(
                    new TerminalLexerRule(terminal, terminal.ToString())));
        }

        public static implicit operator RuleBuilder(BaseLexerRule lexerRule)
        {
            return new RuleBuilder(
                new SymbolBuilder(lexerRule));
        }
        
        public static implicit operator RuleBuilder(ProductionReference productionReference)
        {
            return new RuleBuilder(
                productionReference);
        }
    }
}