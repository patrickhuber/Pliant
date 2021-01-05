using Pliant.Utilities;
using System;

namespace Pliant.Languages.Regex
{
    public class RegexCharacterClass : RegexNode
    {
        public RegexCharacterUnitRange CharacterRange { get; private set; }

        public RegexCharacterClass(RegexCharacterUnitRange characterRange)
        {
            CharacterRange = characterRange;
            _hashCode = ComputeHashCode();
        }

        private readonly int _hashCode;

        private int ComputeHashCode()
        {
            return HashCode.Compute(
                    CharacterRange.GetHashCode());
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (!(obj is RegexCharacterClass characterClass))
                return false;
            return characterClass.CharacterRange.Equals(CharacterRange);
        }

        public override RegexNodeType NodeType
        {
            get { return RegexNodeType.RegexCharacterClass; }
        }

        public override string ToString()
        {
            return CharacterRange.ToString();
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
            _hashCode = ComputeHashCode();
        }

        private readonly int _hashCode;

        int ComputeHashCode()
        {
            return HashCode.Compute(
                    CharacterRange.GetHashCode(),
                    CharacterClass.GetHashCode());
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (!(obj is RegexCharacterClassAlteration characterClassList))
                return false;
            return characterClassList.CharacterRange.Equals(CharacterRange)
                && characterClassList.CharacterClass.Equals(CharacterClass);
        }

        public override RegexNodeType NodeType
        {
            get { return RegexNodeType.RegexCharacterClassAlteration; }
        }

        public override string ToString()
        {
            return $"{CharacterRange}{CharacterClass}";
        }
    }
}