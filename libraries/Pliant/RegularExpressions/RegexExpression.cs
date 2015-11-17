namespace Pliant.RegularExpressions
{
    public class RegexExpression
    {
        public override bool Equals(object obj)
        {
            if ((object)obj == null)
                return false;
            if (obj is RegexExpression)
                return true;
            return false;
        }

        public override int GetHashCode()
        {
            return 0;
        }
    }

    public class RegexExpressionTerm : RegexExpression
    {
        public RegexTerm Term { get; set; }

        public RegexExpressionTerm() { }

        public RegexExpressionTerm(RegexTerm term)
        {
            Term = term;
        }

        public override bool Equals(object obj)
        {
            if ((object)obj == null)
                return false;

            var otherRegexExpressionTerm = obj as RegexExpressionTerm;
            if ((object)otherRegexExpressionTerm == null)
                return false;

            return Term.Equals(otherRegexExpressionTerm.Term);
        }
        
        private bool _isHashCodeSet = false;
        private int _hashCode = 0;

        public override int GetHashCode()
        {
            if (!_isHashCodeSet)
            {
                _hashCode = HashUtil.ComputeHash(Term.GetHashCode());
                _isHashCodeSet = true;
            }
            return _hashCode;
        }
    }

    public class RegexExpressionAlteration : RegexExpressionTerm
    {
        public RegexExpression Expression { get; private set; }

        public RegexExpressionAlteration() 
            : base() { }

        public RegexExpressionAlteration(RegexTerm term, RegexExpression expression)
            : base(term)
        {
            Expression = expression;
        }

        private bool _isHashCodeSet = false;
        private int _hashCode = 0;

        public override int GetHashCode()
        {
            if (!_isHashCodeSet)
            {
                _hashCode = HashUtil.ComputeHash(
                    Term.GetHashCode(),
                    Expression.GetHashCode());
                _isHashCodeSet = true;
            }
            return _hashCode;
        }

        public override bool Equals(object obj)
        {
            if ((object)obj == null)
                return false;

            var otherAlteration = obj as RegexExpressionAlteration;
            if ((object)otherAlteration == null)
                return false;

            return otherAlteration.Expression.Equals(Expression);
        }
    }
}