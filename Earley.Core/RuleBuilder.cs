using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Earley
{
    public class RuleBuilder : Earley.IRuleBuilder
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
                        var terminal = new Terminal(symbol.ToString());
                        symbolList.Add(terminal);
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
