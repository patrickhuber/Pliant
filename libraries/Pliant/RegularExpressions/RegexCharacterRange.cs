using System;

namespace Pliant.RegularExpressions
{
    public class RegexCharacterUnitRange : RegexNode
    {
        public RegexCharacterClassCharacter StartCharacter { get; set; }

        public RegexCharacterUnitRange(RegexCharacterClassCharacter startCharacter)
        {
            StartCharacter = startCharacter;
            _hashCode = new Lazy<int>(ComputeHash);
        }

        public override bool Equals(object obj)
        {
            if ((object)obj == null)
                return false;

            var characterRange = obj as RegexCharacterUnitRange;
            if ((object)characterRange == null)
                return false;

            return characterRange.StartCharacter.Equals(StartCharacter);
        }

        private readonly Lazy<int> _hashCode;

        private int ComputeHash()
        {
            return HashUtil.ComputeHash(
                    StartCharacter.GetHashCode());
        }

        public override int GetHashCode()
        {
            return _hashCode.Value;
        }

        public override RegexNodeType NodeType
        {
            get { return RegexNodeType.RegexCharacterUnitRange; }
        }
    }

    public class RegexCharacterRange : RegexCharacterUnitRange
    {
        public RegexCharacterClassCharacter EndCharacter { get; set; }

        public RegexCharacterRange(
            RegexCharacterClassCharacter startCharacter,
            RegexCharacterClassCharacter endCharacter)
            : base(startCharacter)
        {
            EndCharacter = endCharacter;
            _hashCode = new Lazy<int>(ComputeHash);
        }

        public override bool Equals(object obj)
        {
            if ((object)obj == null)
                return false;
            var characterRangeSet = obj as RegexCharacterRange;
            if ((object)characterRangeSet == null)
                return false;
            return
                StartCharacter.Equals(characterRangeSet.StartCharacter)
                && EndCharacter.Equals(characterRangeSet.EndCharacter);
        }
        
        private readonly Lazy<int> _hashCode;

        int ComputeHash()
        {
            return HashUtil.ComputeHash(
                    StartCharacter.GetHashCode(),
                    EndCharacter.GetHashCode());
        }

        public override int GetHashCode()
        {
            return _hashCode.Value;
        }

        public override RegexNodeType NodeType
        {
            get { return RegexNodeType.RegexCharacterRange; }
        }
    }
}