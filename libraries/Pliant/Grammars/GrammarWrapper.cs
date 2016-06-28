using System.Collections.Generic;

namespace Pliant.Grammars
{
    public abstract class GrammarWrapper : IGrammar
    {
        private IGrammar _innerGrammar;

        protected GrammarWrapper(IGrammar innerGrammar)
        {
            _innerGrammar = innerGrammar;
        }

        public IReadOnlyList<IProduction> Productions
        {
            get { return _innerGrammar.Productions; }
        }

        public INonTerminal Start
        {
            get { return _innerGrammar.Start; }
        }

        public IReadOnlyList<ILexerRule> Ignores
        {
            get { return _innerGrammar.Ignores; }
        }

        public IReadOnlyList<IProduction> RulesFor(INonTerminal nonTerminal)
        {
            return _innerGrammar.RulesFor(nonTerminal);
        }

        public IReadOnlyList<IProduction> StartProductions()
        {
            return _innerGrammar.StartProductions();
        }

        public bool IsNullable(INonTerminal nonTerminal)
        {
            return _innerGrammar.IsNullable(nonTerminal);
        }
    }
}
