namespace Pliant.RegularExpressions
{
    public class RegexCharacter
    {
        public char Value { get; set; }
        
        public RegexCharacter(char value)
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
            var character = obj as RegexCharacter;
            if ((object)character == null)
                return false;
            return character.Value.Equals(Value);
        }
    }
}