namespace Pliant.RegularExpressions
{
    public class RegexExpression
    {
    }

    public class RegexExpressionTerm : RegexExpression
    {
        public RegexTerm Term { get; set; }
    }

    public class RegexExpresssionAlteration : RegexExpressionTerm
    {
        public RegexExpression Expression { get; set; }
    }
}