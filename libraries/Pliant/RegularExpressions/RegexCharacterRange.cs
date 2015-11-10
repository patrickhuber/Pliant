namespace Pliant.RegularExpressions
{
    public class RegexCharacterRange
    {
        public RegexCharacterClassCharacter StartCharacter { get; set; }
    }

    public class RegexCharacterRangeSet : RegexCharacterRange
    {
        public RegexCharacterClassCharacter EndCharacter { get; set; }
    }
}