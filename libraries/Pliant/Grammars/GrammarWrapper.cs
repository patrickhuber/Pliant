using System;
using System.Collections.Generic;

namespace Pliant.Grammars
{
    public abstract class GrammarWrapper : IGrammar
    {
        private readonly IGrammar _innerGrammar;

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

        public IReadOnlyList<ILexerRule> Trivia
        {
            get { return _innerGrammar.Trivia; }
        }

        public IReadOnlyList<ILexerRule> LexerRules
        {
            get { return _innerGrammar.LexerRules; }
        }

        public IReadOnlyDottedRuleRegistry DottedRules
        {
            get { return _innerGrammar.DottedRules; }
        }

        public int GetLexerRuleIndex(ILexerRule lexerRule)
        {
            return _innerGrammar.GetLexerRuleIndex(lexerRule);
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

        public bool IsTransativeNullable(INonTerminal nonTerminal)
        {
            return _innerGrammar.IsTransativeNullable(nonTerminal);
        }

        public IReadOnlyList<IProduction> RulesContainingSymbol(INonTerminal nonTerminal)
        {
            return _innerGrammar.RulesContainingSymbol(nonTerminal);
        }

        public bool IsRightRecursive(ISymbol symbol)
        {
            return _innerGrammar.IsRightRecursive(symbol);
        }
    }
}
