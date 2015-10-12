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
        private IList<ILexerRule> _ignoreRules;
        private string _start;

        public GrammarBuilder(string start)
        {
            _start = start;
            _productions = new List<IProduction>();
            _ignoreRules = new List<ILexerRule>();
        }

        public GrammarBuilder(ProductionBuilder start, ProductionBuilder[] productionBuilder)
            : this(start.LeftHandSide.Value)
        {
            foreach (var production in productionBuilder)
            {
                foreach (var alteration in production.Alterations)
                {
                    _productions.Add(new Production(production.LeftHandSide, alteration.ToArray()));
                }                
            }
        }

        public IGrammar ToGrammar()
        {
            if (_start == null)
                throw new Exception("no start production specified");

            // PERF: Avoid Linq FirstOrDefault due to lambda allocation
            IProduction startProduction = null;
            foreach (var production in _productions)
                if (production.LeftHandSide.Value == _start)
                {
                    startProduction = production;
                    break;
                }

            if (startProduction == null)
                throw new Exception("no start production found for start symbol");

            var start = startProduction.LeftHandSide;

            return new Grammar(
                start, 
                _productions.ToArray(), 
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
                
        public IGrammarBuilder Ignore(ILexerRule lexerRule)
        {
            _ignoreRules.Add(lexerRule);
            return this;
        }
    }
}
