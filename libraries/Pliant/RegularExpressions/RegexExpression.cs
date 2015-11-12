namespace Pliant.RegularExpressions
{
    public class RegexExpression
    {
    }

    public class RegexExpressionTerm : RegexExpression
    {
        public RegexTerm Term { get; set; }
    }

    public class RegexExpressionAlteration : RegexExpressionTerm
    {
        public RegexExpression Expression { get; set; }
    }
}