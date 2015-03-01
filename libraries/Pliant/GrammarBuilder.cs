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
        private IList<ILexeme> _lexemes;
        private IList<string> _ignore;
        private string _start;

        public GrammarBuilder()
        {
            _productions = new List<IProduction>();
            _lexemes = new List<ILexeme>();
            _ignore = new List<string>();
        }

        public GrammarBuilder(Action<IGrammarBuilder> grammar)
        {
            var grammarBuilder = new GrammarBuilder();
            grammar(grammarBuilder);
            _productions = grammarBuilder.GetProductions();
            _lexemes = grammarBuilder.GetLexemes();
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
        
        public Grammar GetGrammar()
        {
            return new Grammar(_productions.ToArray(), _lexemes.ToArray());
        }

        internal IList<IProduction> GetProductions()
        {
            return _productions;
        }

        internal IList<ILexeme> GetLexemes()
        {
            return _lexemes;
        }

        public IGrammarBuilder Start(string name)
        {
            _start = name;
            return this;
        }

        public IGrammarBuilder Lexeme(string name, Action<ITerminalBuilder> terminals)
        {
            var terminalBuilder = new TerminalBuilder();
            terminals(terminalBuilder);
            var lexeme = new Lexeme(new NonTerminal(name), terminalBuilder.GetTerminals().ToArray());
            _lexemes.Add(lexeme);
            return this;
        }

        public IGrammarBuilder Ignore(string name)
        {
            _ignore.Add(name);
            return this;
        }
    }
}
