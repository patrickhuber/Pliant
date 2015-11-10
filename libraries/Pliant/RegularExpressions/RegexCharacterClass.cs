namespace Pliant.RegularExpressions
{
    public class RegexCharacterClass
    {
        public RegexCharacterRange CharacterRange { get; set; }
    }

    public class RegexCharacterClassList : RegexCharacterClass
    {
        public RegexCharacterClass CharacterClass { get; set; }
    }
}