using Pliant.Utilities;
using System;

namespace Pliant.RegularExpressions
{
    public class RegexCharacter : RegexNode
    {
        private readonly int _hashCode;

        public char Value { get; private set; }

        public bool IsEscaped { get; private set; }

        public override RegexNodeType NodeType
        {
            get { return RegexNodeType.RegexCharacterClassCharacter; }
        }

        public RegexCharacter(char value, bool isEscaped = false)
        {
            Value = value;
            IsEscaped = isEscaped;
            _hashCode = ComputeHashCode();
        }

        private int ComputeHashCode()
        {
            return HashCode.Compute(
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
            var regexCharacter = obj as RegexCharacter;
            if ((object)regexCharacter == null)
                return false;
            return regexCharacter.Value.Equals(Value) 
                && regexCharacter.IsEscaped.Equals(IsEscaped);
        }

        public override string ToString()
        {
            if (IsEscaped)
                return $"\\{Value}";
            return Value.ToString();
        }
    }
}