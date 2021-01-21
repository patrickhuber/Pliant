using System;

namespace Pliant.Grammars
{
    public class LexerRuleType
    {
        public string Id { get; private set; }
        private readonly int _hashCode;

        public LexerRuleType(string id)
        {
            Id = id;
            _hashCode = ComputeHashCode(id);
        }

        private static int ComputeHashCode(string id)
        {
            return id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;

            if (!(obj is LexerRuleType other))
                return false;

            return other.Id.Equals(Id, StringComparison.Ordinal);
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public static bool operator ==(LexerRuleType first, LexerRuleType second)
        {
            return first.Equals(second);
        }

        public static bool operator !=(LexerRuleType first, LexerRuleType second)
        {
            return !first.Equals(second);
        }
    }
}