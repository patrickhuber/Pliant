namespace Pliant.RegularExpressions
{
    public class RegexCharacterClass : RegexNode
    {
        public RegexCharacterRange CharacterRange { get; private set; }

        public RegexCharacterClass(RegexCharacterRange characterRange)
        {
            CharacterRange = characterRange;
        }

        private bool _isHashCodeSet = false;
        private int _hashCode = 0;

        public override int GetHashCode()
        {
            if (!_isHashCodeSet)
            {
                _hashCode = HashUtil.ComputeHash(
                    CharacterRange.GetHashCode());
                _isHashCodeSet = true;
            }
            return _hashCode;
        }

        public override bool Equals(object obj)
        {
            if ((object)obj == null)
                return false;
            var characterClass = obj as RegexCharacterClass;
            if ((object)characterClass == null)
                return false;
            return characterClass.CharacterRange.Equals(CharacterRange);
        }

        public override RegexNodeType NodeType
        {
            get { return RegexNodeType.RegexCharacterClass; }
        }
    }

    public class RegexCharacterClassList : RegexCharacterClass
    {
        public RegexCharacterClass CharacterClass { get; private set; }

        public RegexCharacterClassList(
            RegexCharacterRange characterRange,
            RegexCharacterClass characterClass)
            : base(characterRange)
        {
            CharacterClass = characterClass;
        }

        private bool _isHashCodeSet = false;
        private int _hashCode = 0;

        public override int GetHashCode()
        {
            if (!_isHashCodeSet)
            {
                _hashCode = HashUtil.ComputeHash(
                    CharacterRange.GetHashCode(),
                    CharacterClass.GetHashCode());
                _isHashCodeSet = true;
            }
            return _hashCode;
        }

        public override bool Equals(object obj)
        {
            if ((object)obj == null)
                return false;
            var characterClassList = obj as RegexCharacterClassList;
            if ((object)characterClassList == null)
                return false;
            return characterClassList.CharacterRange.Equals(CharacterRange)
                && characterClassList.CharacterClass.Equals(CharacterClass);
        }

        public override RegexNodeType NodeType
        {
            get { return RegexNodeType.RegexCharacterClassList; }
        }
    }
}