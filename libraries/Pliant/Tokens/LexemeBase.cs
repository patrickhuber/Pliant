using Pliant.Captures;
using Pliant.Grammars;
using Pliant.Utilities;
using System.Collections.Generic;

namespace Pliant.Tokens
{
    public abstract class LexemeBase<TLexerRule> : ILexeme
        where TLexerRule : ILexerRule
    {
        private static readonly ITrivia[] EmptyTriviaArray = { };
        private List<ITrivia> _leadingTrivia;
        private List<ITrivia> _trailingTrivia;

        protected TLexerRule ConcreteLexerRule { get; private set; }

        public IReadOnlyList<ITrivia> LeadingTrivia
        {
            get
            {
                if (_leadingTrivia is null)
                    return EmptyTriviaArray;
                return _leadingTrivia;
            }
        }

        public IReadOnlyList<ITrivia> TrailingTrivia
        {
            get
            {
                if (_trailingTrivia is null)
                    return EmptyTriviaArray;
                return _trailingTrivia;
            }
        }

        public ILexerRule LexerRule => ConcreteLexerRule;
                
        public ICapture<char> Capture { get; private set; }
        
        public int Position => Capture.Offset;

        public TokenType TokenType
        {
            get { return LexerRule.TokenType; }
        }

        protected LexemeBase(TLexerRule lexerRule, ICapture<char> parentSegment, int offset)
        {
            ConcreteLexerRule = lexerRule;
            Capture = parentSegment.Slice(offset, 0);
        }
        
        public abstract bool IsAccepted();

        public abstract bool Scan();

        public void AddTrailingTrivia(ITrivia trivia)
        {
            if (_trailingTrivia is null)
            {
                var pool = SharedPools.Default<List<ITrivia>>();
                _trailingTrivia = pool.AllocateAndClear();
            }

            _trailingTrivia.Add(trivia);
        }

        public void AddLeadingTrivia(ITrivia trivia)
        {
            if (_leadingTrivia is null)
            {
                var pool = SharedPools.Default<List<ITrivia>>();
                _leadingTrivia = pool.AllocateAndClear();
            }
            _leadingTrivia.Add(trivia);
        }
        
        public abstract void Reset();

        public virtual void Reset(TLexerRule lexerRule, int offset)
        {
            ResetInternal(lexerRule, offset);
            Reset();
        }

        protected void ResetInternal(TLexerRule lexerRule, int offset)
        {
            var pool = SharedPools.Default<List<ITrivia>>();
            if (_leadingTrivia != null)
                pool.ClearAndFree(_leadingTrivia);
            if (_trailingTrivia != null)
                pool.ClearAndFree(_trailingTrivia);
            ConcreteLexerRule = lexerRule;
            Capture.Offset = offset;
            Capture.Count = 0;
        }
    }
}
