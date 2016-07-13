using Pliant.Utilities;
using System;

namespace Pliant.Tokens
{
    public class Token : IToken
    {
        public string Value { get; private set; }

        public int Origin { get; private set; }

        public TokenType TokenType { get; private set; }

        public Token(string value, int origin, TokenType tokenType)
        {
            Value = value;
            Origin = origin;
            TokenType = tokenType;
            _hashCode = ComputeHashCode();
        }

        private int ComputeHashCode()
        {
            return HashCode.Compute(
                TokenType.GetHashCode(), 
                Origin.GetHashCode(), 
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
                && Origin == token.Origin
                && TokenType.Equals(token.TokenType);
        }
    }
}