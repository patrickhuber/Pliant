namespace Pliant.RegularExpressions
{
    public class RegexCharacterRange
    {
        public RegexCharacterClassCharacter StartCharacter { get; set; }

        public RegexCharacterRange() { }

        public RegexCharacterRange(RegexCharacterClassCharacter startCharacter)
        {
            StartCharacter = startCharacter;
        }
    }

    public class RegexCharacterRangeSet : RegexCharacterRange
    {
        public RegexCharacterClassCharacter EndCharacter { get; set; }

        public RegexCharacterRangeSet() { }
        public RegexCharacterRangeSet(
            RegexCharacterClassCharacter startCharacter, 
            RegexCharacterClassCharacter endCharacter)
            : base(startCharacter)
        {
            EndCharacter = endCharacter;
        }


        public override bool Equals(object obj)
        {
            if ((object)obj == null)
                return false;
            var characterRangeSet = obj as RegexCharacterRangeSet;
            if ((object)characterRangeSet == null)
                return false;
            return
                StartCharacter.Equals(characterRangeSet.StartCharacter)
                && EndCharacter.Equals(characterRangeSet.EndCharacter);
        }
        
        private bool _isHashCodeSet = false;
        private int _hashCode = 0;

        public override int GetHashCode()
        {
            if (!_isHashCodeSet)
            {
                _hashCode = HashUtil.ComputeHash(
                    StartCharacter.GetHashCode(),
                    EndCharacter.GetHashCode());
                _isHashCodeSet = true;
            }
            return _hashCode;
        }
    }
}