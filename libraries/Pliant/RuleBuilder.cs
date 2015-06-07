using Pliant.Grammars;
using Pliant.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pliant
{
    public class RuleBuilder : IRuleBuilder
    {
        private IList<IList<ISymbol>> _rules;
        
        public RuleBuilder()
        {
            _rules = new List<IList<ISymbol>>();
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
                        var terminalLexerRule = new TerminalLexerRule(
                            terminal, 
                            new TokenType(terminal.ToString()));
                        // TODO: add the terminalLexerRule instead of the Terminal
                        symbolList.Add(terminal);
                    }
                    else if (symbol is ITerminal)
                    {
                        symbolList.Add(symbol as ITerminal);
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
