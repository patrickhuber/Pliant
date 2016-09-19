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
            if (((object)obj) == null)
                return false;

            var other = obj as LexerRuleType;

            if (((object)other) == null)
                return false;

            return other.Id == Id;
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