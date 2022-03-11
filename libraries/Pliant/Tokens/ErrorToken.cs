using Pliant.Automata;
using Pliant.Captures;
using Pliant.Grammars;
using Pliant.Utilities;
using System.Collections.Generic;

namespace Pliant.Tokens
{
    public class ErrorToken : ILexeme
    {
        private static readonly AnyLexerRule AnyLexerRule = new AnyLexerRule("Pliant.Tokens.Error");

        private static readonly ITrivia[] EmptyTriviaArray = { };
        private List<ITrivia> _leadingTrivia;
        private List<ITrivia> _trailingTrivia;

        public ILexerRule LexerRule => AnyLexerRule;

        public ICapture<char> Capture { get; private set; }

        public int Position => Capture.Offset;

        public TokenType TokenType { get; private set; }

        public ErrorToken(TokenType tokenType, ICapture<char> parentCapture, int offset)
        {
            TokenType = tokenType;
            Capture = parentCapture.Slice(offset, 0);
        }
        
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

        public bool IsAccepted()
        {
            return true;
        }

        public void Reset()
        {
            var pool = SharedPools.Default<List<ITrivia>>();
            if (_leadingTrivia != null)
            {
                pool.ClearAndFree(_leadingTrivia);
                _leadingTrivia = null;
            }
            if (_trailingTrivia != null)
            {
                pool.ClearAndFree(_trailingTrivia);
                _trailingTrivia = null;
            }
            Capture.Count = 0;
        }

        public virtual void Reset(TokenType tokenType, int offset)
        {
            TokenType = tokenType;
            Capture.Offset = offset;
            Capture.Count = 0;
        }

        public bool Scan()
        {
            return Capture.Grow();
        }
    }
}
