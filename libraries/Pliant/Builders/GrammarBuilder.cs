using Pliant.Grammars;
using Pliant.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pliant.Builders
{
    public class GrammarBuilder : IGrammarBuilder
    {
        private IList<IProduction> _productions;
        private IList<ILexerRule> _lexerRules;
        private IList<ILexerRule> _ignoreRules;
        private string _start;

        public GrammarBuilder(string start)
        {
            _start = start;
            _productions = new List<IProduction>();
            _lexerRules = new List<ILexerRule>();
            _ignoreRules = new List<ILexerRule>();
        }
                        
        public IGrammar ToGrammar()
        {
            if (_start == null)
                throw new Exception("no start production specified");
            var startProduction = _productions.FirstOrDefault(x => x.LeftHandSide.Value == _start);
            if (startProduction == null)
                throw new Exception("no start production found for start symbol");
            var start = startProduction.LeftHandSide;

            return new Grammar(
                start, 
                _productions.ToArray(), 
                _lexerRules.ToArray(), 
                _ignoreRules.ToArray());
        }

        public IGrammarBuilder Production(string name, Action<IRuleBuilder> rules)
        {
            if (rules == null)
                _productions.Add(new Production(name));
            else
            {
                var ruleBuilder = new RuleBuilder();
                rules(ruleBuilder);
                foreach (var rule in ruleBuilder.GetRules())
                {
                    var production = new Production(name, rule.ToArray());
                    _productions.Add(production);
                }
            }
            return this;
        }

        public IGrammarBuilder LexerRule(string name, ITerminal terminal)
        {
            var lexerRule = new TerminalLexerRule(terminal, new TokenType(name));
            _lexerRules.Add(lexerRule);
            return this;
        }

        public IGrammarBuilder LexerRule(ILexerRule lexerRule)
        {
            _lexerRules.Add(lexerRule);
            return this;
        }
        
        public IGrammarBuilder Ignore(ILexerRule lexerRule)
        {
            _ignoreRules.Add(lexerRule);
            return this;
        }
    }
}
