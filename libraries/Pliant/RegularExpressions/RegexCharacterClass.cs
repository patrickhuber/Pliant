using System;

namespace Pliant.RegularExpressions
{
    public class RegexCharacterClass : RegexNode
    {
        public RegexCharacterRange CharacterRange { get; private set; }

        public RegexCharacterClass(RegexCharacterRange characterRange)
        {
            CharacterRange = characterRange;
            _hashCode = new Lazy<int>(ComputeHash);
        }

        private readonly Lazy<int> _hashCode;

        private int ComputeHash()
        {
            return HashUtil.ComputeHash(
                    CharacterRange.GetHashCode());
        }

        public override int GetHashCode()
        {
            return _hashCode.Value;
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
            _hashCode = new Lazy<int>(ComputeHash);
        }

        private readonly Lazy<int> _hashCode;

        int ComputeHash()
        {
            return HashUtil.ComputeHash(
                    CharacterRange.GetHashCode(),
                    CharacterClass.GetHashCode());
        }

        public override int GetHashCode()
        {
            return _hashCode.Value;
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