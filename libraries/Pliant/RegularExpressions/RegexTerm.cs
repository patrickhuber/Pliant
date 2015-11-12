namespace Pliant.RegularExpressions
{
    public class RegexTerm
    {
        public RegexFactor Factor { get; set; }
    }

    public class RegexTermFactor : RegexTerm
    {
        public RegexTerm Term { get; set; }
    }
}