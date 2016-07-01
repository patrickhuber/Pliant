using Pliant.Utilities;
using System;

namespace Pliant.RegularExpressions
{
    public class RegexCharacterClassCharacter : RegexNode
    {
        public char Value { get; private set; }

        public override RegexNodeType NodeType
        {
            get { return RegexNodeType.RegexCharacterClassCharacter; }
        }

        public RegexCharacterClassCharacter(char value)
        {
            Value = value;
            _hashCode = ComputeHashCode();
        }
        
        private readonly int _hashCode;

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
            var characterClassCharacter = obj as RegexCharacterClassCharacter;
            if ((object)characterClassCharacter == null)
                return false;
            return characterClassCharacter.Value.Equals(Value);
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}