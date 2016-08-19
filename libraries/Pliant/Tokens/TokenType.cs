using Pliant.Utilities;
using System;

namespace Pliant.Tokens
{
    public class TokenType
    {
        public string Id { get; private set; }
        private readonly int _hashCode;
        public TokenType(string id)
        {
            Id = id;
            _hashCode = ComputeHashCode(Id);
        }

        private static int ComputeHashCode(string id)
        {
            return id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if ((object)obj == null)
                return false;
            var other = obj as TokenType;
            if ((object)other == null)
                return false;
            return other.Id.Equals(Id);
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override string ToString()
        {
            return Id;
        }

        public static bool operator ==(TokenType first, TokenType second)
        {
            return first.Equals(second);
        }

        public static bool operator !=(TokenType first, TokenType second)
        {
            return !(first == second);
        }
    }
}