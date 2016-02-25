using System;

namespace Pliant.RegularExpressions
{
    public class RegexCharacterClass : RegexNode
    {
        public RegexCharacterUnitRange CharacterRange { get; private set; }

        public RegexCharacterClass(RegexCharacterUnitRange characterRange)
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

    public class RegexCharacterClassAlteration : RegexCharacterClass
    {
        public RegexCharacterClass CharacterClass { get; private set; }

        public RegexCharacterClassAlteration(
            RegexCharacterUnitRange characterRange,
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
            var characterClassList = obj as RegexCharacterClassAlteration;
            if ((object)characterClassList == null)
                return false;
            return characterClassList.CharacterRange.Equals(CharacterRange)
                && characterClassList.CharacterClass.Equals(CharacterClass);
        }

        public override RegexNodeType NodeType
        {
            get { return RegexNodeType.RegexCharacterClassAlteration; }
        }
    }
}