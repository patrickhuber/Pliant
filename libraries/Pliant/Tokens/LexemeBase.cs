using Pliant.Grammars;
using Pliant.Utilities;
using System.Collections.Generic;

namespace Pliant.Tokens
{
    public abstract class LexemeBase<TLexerRule> : ILexeme
    {
        private static readonly ITrivia[] EmptyTriviaArray = { };
        private List<ITrivia> _leadingTrivia;
        private List<ITrivia> _trailingTrivia;

        public IReadOnlyList<ITrivia> LeadingTrivia
        {
            get
            {
                if (_leadingTrivia == null)
                    return EmptyTriviaArray;
                return _leadingTrivia;
            }
        }

        public IReadOnlyList<ITrivia> TrailingTrivia
        {
            get
            {
                if (_trailingTrivia == null)
                    return EmptyTriviaArray;
                return _trailingTrivia;
            }
        }

        public ILexerRule LexerRule { get; private set; }

        protected TLexerRule ConcreteLexerRule { get; private set; }

        public abstract string Value { get; }

        public int Position { get; private set; }

        public TokenType TokenType
        {
            get { return LexerRule.TokenType; }
        }

        protected LexemeBase(TLexerRule lexerRule, int position)
        {
            LexerRule = lexerRule as ILexerRule;
            ConcreteLexerRule = lexerRule;
            Position = position;
        }
        
        public abstract bool IsAccepted();

        public abstract bool Scan(char c);

        public void AddTrailingTrivia(ITrivia trivia)
        {
            if (_trailingTrivia == null)
            {
                var pool = SharedPools.Default<List<ITrivia>>();
                _trailingTrivia = pool.AllocateAndClear();
            }

            _trailingTrivia.Add(trivia);
        }

        public void AddLeadingTrivia(ITrivia trivia)
        {
            if (_leadingTrivia == null)
            {
                var pool = SharedPools.Default<List<ITrivia>>();
                _leadingTrivia = pool.AllocateAndClear();
            }
            _leadingTrivia.Add(trivia);
        }
        
        public abstract void Reset();

        public virtual void Reset(TLexerRule lexerRule, int position)
        {
            ResetInternal(lexerRule, position);
            Reset();
        }

        protected void ResetInternal(TLexerRule lexerRule, int position)
        {
            var pool = SharedPools.Default<List<ITrivia>>();
            if(_leadingTrivia != null)
                pool.ClearAndFree(_leadingTrivia);
            if(_trailingTrivia != null)
                pool.ClearAndFree(_trailingTrivia);
            LexerRule = lexerRule as ILexerRule;
            ConcreteLexerRule = lexerRule;
            Position = position;
        }
    }
}
