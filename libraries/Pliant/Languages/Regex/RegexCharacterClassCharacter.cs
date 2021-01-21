using Pliant.Utilities;
using System;

namespace Pliant.Languages.Regex
{
    public class RegexCharacterClassCharacter : RegexNode
    {
        private readonly int _hashCode;

        public char Value { get; private set; }

        public bool IsEscaped { get; private set; }

        public override RegexNodeType NodeType
        {
            get { return RegexNodeType.RegexCharacterClassCharacter; }
        }
        
        public RegexCharacterClassCharacter(char value, bool isEscaped = false)
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
            if (obj is null)
                return false;
            if (!(obj is RegexCharacterClassCharacter characterClassCharacter))
                return false;
            return characterClassCharacter.Value.Equals(Value)
                && characterClassCharacter.IsEscaped.Equals(IsEscaped);
        }

        public override string ToString()
        {
            if (IsEscaped)
                return $"\\{Value}";
            return Value.ToString();
        }
    }
}