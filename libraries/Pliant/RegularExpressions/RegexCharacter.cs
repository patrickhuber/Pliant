using System;

namespace Pliant.RegularExpressions
{
    public class RegexCharacter : RegexNode
    {
        public char Value { get; set; }

        public override RegexNodeType NodeType
        {
            get { return RegexNodeType.RegexCharacter; }
        }

        public RegexCharacter(char value)
        {
            Value = value;
            _hashCode = ComputeHashCode();
        }
        
        private readonly int _hashCode;

        int ComputeHashCode()
        {
            return HashUtil.ComputeHash(
                    Value.GetHashCode());
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }
        
        public override bool Equals(object obj)
        {
            if ((object)obj == null)
                return false;
            var character = obj as RegexCharacter;
            if ((object)character == null)
                return false;
            return character.Value.Equals(Value);
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}