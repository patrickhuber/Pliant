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

        private IGrammarLexerRule CreateGrammarLexerRule(IList<ITerminal> terminalNeighborList)
        {
            var startNonTerminal = new NonTerminal("S");
            var production = new Production(
                startNonTerminal, 
                terminalNeighborList.ToArray());
            var grammar = new Grammar(startNonTerminal, new[] { production },  null);
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
