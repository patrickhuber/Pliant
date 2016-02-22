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
            _hashCode = new Lazy<int>(ComputeHash);
        }
        
        private readonly Lazy<int> _hashCode;

        private int ComputeHash()
        {
            return HashUtil.ComputeHash(
                    Value.GetHashCode());
        }

        public override int GetHashCode()
        {
            return _hashCode.Value;
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
    }
}