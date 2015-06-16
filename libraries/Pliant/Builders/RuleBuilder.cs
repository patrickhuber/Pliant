using Pliant.Collections;
using Pliant.Grammars;
using Pliant.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pliant.Builders
{
    public class RuleBuilder : IRuleBuilder
    {
        private IList<IList<ISymbol>> _rules;
        
        public RuleBuilder()
        {
            _rules = new List<IList<ISymbol>>();
        }

        public IRuleBuilder Rule1(params object[] symbols)
        {
            var symbolList = new List<ISymbol>();
            var terminalNeighborList = new List<ITerminal>();

            if (symbols.IsNullOrEmpty())
                return this;

            foreach (var symbol in symbols)
            {
                var flushTerminals = false;
                if (symbol is INonTerminal)
                {
                    flushTerminals = true;
                    symbolList.Add(symbol as INonTerminal);
                }
                else if (symbol is string)
                {
                    flushTerminals = true;
                    var nonTerminal = new NonTerminal(symbol as string);
                    symbolList.Add(nonTerminal);
                }
                else if (symbol is char)
                {
                    var terminal = new Terminal((char)symbol);
                    terminalNeighborList.Add(terminal);
                }
                else if (symbol is ITerminal)
                {
                    terminalNeighborList.Add(symbol as ITerminal);
                }
                else if (symbol is ILexerRule)
                {
                    flushTerminals = true;
                    symbolList.Add(symbol as ILexerRule);
                }
                else if (symbol == null)
                { }
                else { throw new ArgumentException("unrecognized terminal or nonterminal"); }
                
                if (flushTerminals)
                {
                    var grammarLexerRule = CreateGrammarLexerRule(terminalNeighborList);
                    symbolList.Add(grammarLexerRule);
                    terminalNeighborList.Clear();
                }
            }

            if (terminalNeighborList.Count > 0)
            {
                var grammarLexerRule = CreateGrammarLexerRule(terminalNeighborList);
                symbolList.Add(grammarLexerRule);
                terminalNeighborList.Clear();
            }

            _rules.Add(symbolList);
            return this;
        }

        private IGrammarLexerRule CreateGrammarLexerRule(IList<ITerminal> terminalNeighborList)
        {
            var startNonTerminal = new NonTerminal("S");
            var production = new Production(
                startNonTerminal, 
                terminalNeighborList.ToArray());
            var grammar = new Grammar(startNonTerminal, new[] { production }, null, null);
            return new GrammarLexerRule(Guid.NewGuid().ToString(), grammar);
        }

        public IRuleBuilder Rule(params object[] symbols)
        {
            var symbolList = new List<ISymbol>();
            if (symbols != null)
            {
                foreach (var symbol in symbols)
                {
                    if (symbol is char)
                    {
                        var terminal = new Terminal((char)symbol);
                        var lexerRule = new TerminalLexerRule(
                            terminal,
                            new TokenType(terminal.ToString()));
                        symbolList.Add(lexerRule);
                    }
                    else if (symbol is ITerminal)
                    {
                        var terminal = symbol as ITerminal;
                        var lexerRule = new TerminalLexerRule(
                            terminal,
                            new TokenType(terminal.ToString()));
                        symbolList.Add(lexerRule);
                    }
                    else if (symbol is ILexerRule)
                    {
                        symbolList.Add(symbol as ILexerRule);
                    }
                    else if (symbol is string)
                    {
                        var nonTerminal = new NonTerminal(symbol as string);
                        symbolList.Add(nonTerminal);
                    }
                    else if (symbol == null)
                    { }
                    else { throw new ArgumentException("unrecognized terminal or nonterminal"); }
                }
            }
            _rules.Add(symbolList);
            return this;
        }

        public IRuleBuilder Lambda()
        {
            return Rule();
        }

        public IList<IList<ISymbol>> GetRules()
        {
            return _rules.ToArray();
        }
    }
}
