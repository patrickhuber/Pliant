using System;

namespace Pliant.RegularExpressions
{
    public class RegexExpression : RegexNode
    {
        public override bool Equals(object obj)
        {
            if ((object)obj == null)
                return false;

            var otherRegexExpression = obj as RegexExpression;
            if (otherRegexExpression != null)
                return false;
            return otherRegexExpression.NodeType == RegexNodeType.RegexExpression;
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public override RegexNodeType NodeType
        {
            get
            {
                return RegexNodeType.RegexExpression;
            }
        }
    }

    public class RegexExpressionTerm : RegexExpression
    {
        public RegexTerm Term { get; private set; }

        public RegexExpressionTerm(RegexTerm term)
        {
            Term = term;
            _hashCode = new Lazy<int>(ComputeHash);
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
        
        private readonly Lazy<int> _hashCode;

        int ComputeHash()
        {
            return HashUtil.ComputeHash(Term.GetHashCode());
        }

        public override int GetHashCode()
        {
            return _hashCode.Value;
        }

        public override RegexNodeType NodeType
        {
            get
            {
                return RegexNodeType.RegexExpressionTerm;
            }
        }
    }

    public class RegexExpressionAlteration : RegexExpressionTerm
    {
        public RegexExpression Expression { get; private set; }

        public RegexExpressionAlteration(RegexTerm term, RegexExpression expression)
            : base(term)
        {
            Expression = expression;
            _hashCode = new Lazy<int>(ComputeHash);
        }
        
        private readonly Lazy<int> _hashCode;

        int ComputeHash()
        {
            return HashUtil.ComputeHash(
                Term.GetHashCode(),
                Expression.GetHashCode());
        }

        public override int GetHashCode()
        {
            return _hashCode.Value;
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

        public override RegexNodeType NodeType
        {
            get
            {
                return RegexNodeType.RegexExpressionAlteration;
            }
        }
    }
}