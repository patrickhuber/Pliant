using Pliant.Utilities;
using System;

namespace Pliant.RegularExpressions
{
    public abstract class RegexExpression : RegexNode
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
            _hashCode = ComputeHashCode();
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
        
        private readonly int _hashCode;

        int ComputeHashCode()
        {
            return HashCode.Compute(Term.GetHashCode());
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override RegexNodeType NodeType
        {
            get
            {
                return RegexNodeType.RegexExpressionTerm;
            }
        }

        public override string ToString()
        {
            return Term.ToString();
        }
    }

    public class RegexExpressionAlteration : RegexExpressionTerm
    {
        public RegexExpression Expression { get; private set; }

        public RegexExpressionAlteration(RegexTerm term, RegexExpression expression)
            : base(term)
        {
            Expression = expression;
            _hashCode = ComputeHashCode();
        }
        
        private readonly int _hashCode;

        int ComputeHashCode()
        {
            return HashCode.Compute(
                Term.GetHashCode(),
                Expression.GetHashCode());
        }

        public override int GetHashCode()
        {
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

        public override RegexNodeType NodeType
        {
            get
            {
                return RegexNodeType.RegexExpressionAlteration;
            }
        }

        public override string ToString()
        {
            return $"{Term}|{Expression}";
        }
    }
}