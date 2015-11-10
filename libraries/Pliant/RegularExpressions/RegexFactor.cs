namespace Pliant.RegularExpressions
{
    public class RegexFactor
    {
        public RegexAtom Atom { get; set; }
    }

    public class RegexFactorIterator : RegexFactor
    {
        public RegexIterator Iterator { get; set; }
    }
}