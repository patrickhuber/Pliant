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
        }

        private bool _isHashCodeSet = false;
        private int _hashCode = 0;

        public override int GetHashCode()
        {
            if (!_isHashCodeSet)
            {
                _hashCode = HashUtil.ComputeHash(
                    Value.GetHashCode());
                _isHashCodeSet = true;
            }
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
    }
}