using Pliant.Utilities;
using System;

namespace Pliant.Ebnf
{
    public class EbnfExpressionEmpty: EbnfNode
    {
        private readonly int _hashCode;

        public EbnfExpressionEmpty()
        {
            _hashCode = ComputeHashCode();
        }

        public override EbnfNodeType NodeType
        {
            get
            {
                return EbnfNodeType.EbnfExpressionEmpty;
            }
        }

        public override bool Equals(object obj)
        {
            if ((object)obj == null)
                return false;
            var expression = obj as EbnfExpressionEmpty;
            if ((object)expression == null)
                return false;
            return expression.NodeType == NodeType;
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        int ComputeHashCode()
        {
            return HashCode.Compute(
                NodeType.GetHashCode());
        }
    }

    public class EbnfExpression : EbnfExpressionEmpty
    {
        private readonly int _hashCode;

        public EbnfTerm Term { get; private set; }

        public EbnfExpression(EbnfTerm term)
        {
            Term = term;
            _hashCode = ComputeHashCode();
        }

        public override EbnfNodeType NodeType
        {
            get
            {
                return EbnfNodeType.EbnfExpression;
            }
        }

        public override bool Equals(object obj)
        {
            if ((object)obj == null)
                return false;
            var expression = obj as EbnfExpression;
            if ((object)expression == null)
                return false;
            return expression.NodeType == NodeType
                && expression.Term.Equals(Term);
        }

        int ComputeHashCode()
        {
            return HashCode.Compute(
                NodeType.GetHashCode(),
                Term.GetHashCode());
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override string ToString()
        {
            return Term.ToString();
        }
    }

    public class EbnfExpressionAlteration : EbnfExpression
    {
        private readonly int _hashCode;

        public EbnfExpression Expression { get; private set; }

        public EbnfExpressionAlteration(
            EbnfTerm term,
            EbnfExpression expression)
            : base(term)
        {
            Expression = expression;
            _hashCode = ComputeHashCode();
        }

        public override EbnfNodeType NodeType
        {
            get
            {
                return EbnfNodeType.EbnfExpressionAlteration;
            }
        }

        public override bool Equals(object obj)
        {
            if ((object)obj == null)
                return false;
            var expression = obj as EbnfExpressionAlteration;
            if ((object)expression == null)
                return false;
            return expression.NodeType == NodeType
                && expression.Term.Equals(Term)
                && expression.Expression.Equals(Expression);
        }

        int ComputeHashCode()
        {
            return HashCode.Compute(
                NodeType.GetHashCode(),
                Term.GetHashCode(),
                Expression.GetHashCode());
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override string ToString()
        {
            return $"{Term} | {Expression}";
        }
    }
}