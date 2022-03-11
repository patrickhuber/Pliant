using Pliant.Captures;
using Pliant.Diagnostics;
using Pliant.Grammars;
using Pliant.Utilities;
using System.Collections.Generic;

namespace Pliant.Tokens
{
    public class Token : IToken
    {
        private static readonly ITrivia[] EmptyTriviaArray = { };
        
        public int Position { get; private set; }

        public TokenType TokenType { get; private set; }

        public IReadOnlyList<ITrivia> LeadingTrivia { get; private set; }

        public IReadOnlyList<ITrivia> TrailingTrivia { get; private set; }

        public ICapture<char> Capture { get; private set; }
        
        public Token(string value, int position, TokenType tokenType)
        {
            Capture = value.AsCapture();
            Position = position;
            TokenType = tokenType;
            _hashCode = ComputeHashCode();
        }

        public Token(string value, int position, TokenType tokenType, 
            IReadOnlyList<ITrivia> leadingTrivia, 
            IReadOnlyList<ITrivia> trailingTrivia)
            : this(value, position, tokenType)
        {
            Assert.IsNotNull(leadingTrivia, nameof(leadingTrivia));
            Assert.IsNotNull(trailingTrivia, nameof(trailingTrivia));

            LeadingTrivia = new List<ITrivia>(leadingTrivia);
            TrailingTrivia = new List<ITrivia>(trailingTrivia);
        }

        private int ComputeHashCode()
        {
            return HashCode.Compute(
                TokenType.GetHashCode(), 
                Position.GetHashCode(), 
                Capture.GetHashCode());
        }

        private readonly int _hashCode;

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (!(obj is Token token))
                return false;
            return Capture.Equals(token.Capture)
                && Position == token.Position
                && TokenType.Equals(token.TokenType);
        }
    }
}