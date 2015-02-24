using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant
{
    public class GrammarBuilder : IGrammarBuilder
    {
        private IList<IProduction> _productions;
        private IList<ITerminal> _characterClasses;

        public GrammarBuilder()
        {
            _productions = new List<IProduction>();
            _characterClasses = new List<ITerminal>();
        }

        public GrammarBuilder(Action<IGrammarBuilder> grammar)
        {
            var grammarBuilder = new GrammarBuilder();
            grammar(grammarBuilder);
            _productions = grammarBuilder.GetProductions();
            _characterClasses = grammarBuilder.GetCharacterClasses();
        }
                
        public IGrammarBuilder Production(string name, Action<IRuleBuilder> rules=null)
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
        
        public IGrammarBuilder CharacterClass(string name, Action<ITerminalBuilder> terminals)
        {
            var terminalBuilder = new TerminalBuilder();
            terminals(terminalBuilder);
            foreach (var terminal in terminalBuilder.GetTerminals())
                _characterClasses.Add(terminal);
            return this;
        }

        public Grammar GetGrammar()
        {
            return new Grammar(_productions.ToArray());
        }

        internal IList<IProduction> GetProductions()
        {
            return _productions;
        }

        internal IList<ITerminal> GetCharacterClasses()
        {
            return _characterClasses;
        }
    }
}
