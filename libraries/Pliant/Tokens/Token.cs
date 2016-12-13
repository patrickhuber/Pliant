using Pliant.Utilities;
using System;

namespace Pliant.Tokens
{
    public class Token : IToken
    {
        public string Value { get; private set; }

        public int Position { get; private set; }

        public TokenType TokenType { get; private set; }

        public Token(string value, int position, TokenType tokenType)
        {
            Value = value;
            Position = position;
            TokenType = tokenType;
            _hashCode = ComputeHashCode();
        }

        private int ComputeHashCode()
        {
            return HashCode.Compute(
                TokenType.GetHashCode(), 
                Position.GetHashCode(), 
                Value.GetHashCode());
        }

        private readonly int _hashCode;

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            var token = obj as Token;
            if (token == null)
                return false;
            return Value == token.Value
                && Position == token.Position
                && TokenType.Equals(token.TokenType);
        }
    }
}