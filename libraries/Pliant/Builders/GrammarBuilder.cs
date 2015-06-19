using Pliant.Grammars;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pliant.Builders
{
    public class GrammarBuilder : IGrammarBuilder
    {
        private IList<IProduction> _productions;
        private IList<ILexerRule> _lexerRules;
        private IList<string> _actions;
        private string _start;

        public GrammarBuilder(string start)
        {
            _start = start;
            _productions = new List<IProduction>();
            _lexerRules = new List<ILexerRule>();
            _actions = new List<string>();
        }

        public GrammarBuilder(
            string start, 
            Action<IProductionBuilder> productions, 
            Action<ILexerRuleBuilder> lexerRules = null, 
            Action<ICommandBuilder> action = null)
        {
            _start = start;
            
            var productionBuilder = new ProductionBuilder();
            productions(productionBuilder);
            _productions = productionBuilder.GetProductions();

            var lexerRuleBuilder = new LexerRuleBuilder();
            if(lexerRules != null)
                lexerRules(lexerRuleBuilder);
            _lexerRules = lexerRuleBuilder.GetLexerRules();

            var ignoreBuilder = new CommandBuilder();
            if(action != null)
                action(ignoreBuilder);
            _actions = ignoreBuilder.GetIgnoreList();
        }
                
        public Grammar ToGrammar()
        {
            if (_start == null)
                throw new Exception("no start production specified");
            var startProduction = _productions.FirstOrDefault(x => x.LeftHandSide.Value == _start);
            if (startProduction == null)
                throw new Exception("no start production found for start symbol");
            var start = startProduction.LeftHandSide;
            var ignore = _lexerRules
                .Where(x => _actions.Contains(x.TokenType.Id));

            return new Grammar(start, _productions.ToArray(), _lexerRules.ToArray(), ignore.ToArray());
        }

        public GrammarBuilder Production(string name, Action<IRuleBuilder> rules)
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

        public GrammarBuilder LexerRule(ILexerRule lexerRule)
        {
            _lexerRules.Add(lexerRule);
            return this;
        }

        public GrammarBuilder Ignore(string name)
        {
            _actions.Add(name);
            return this;
        }
    }
}
