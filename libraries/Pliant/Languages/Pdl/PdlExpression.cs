using Pliant.Utilities;

namespace Pliant.Languages.Pdl
{

    public class PdlExpressionEmpty : PdlNode
    {
        private readonly int _hashCode;

        public PdlExpressionEmpty()
        {
            _hashCode = ComputeHashCode();
        }

        public override PdlNodeType NodeType => PdlNodeType.PdlExpressionEmpty;

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (!(obj is PdlExpressionEmpty expression))
                return false;
            return expression.NodeType == NodeType;
        }

        public override int GetHashCode() => _hashCode;

        int ComputeHashCode()
        {
            return HashCode.Compute(
                NodeType.GetHashCode());
        }
    }

    public class PdlExpression : PdlExpressionEmpty
    {
        private readonly int _hashCode;

        public PdlTerm Term { get; private set; }

        public PdlExpression(PdlTerm term)
        {
            Term = term;
            _hashCode = ComputeHashCode();
        }

        public override PdlNodeType NodeType => PdlNodeType.PdlExpression;

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (!(obj is PdlExpression expression))
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

        public override int GetHashCode() => _hashCode;

        public override string ToString() => Term.ToString();
    }

    public class PdlExpressionAlteration : PdlExpression
    {
        private readonly int _hashCode;

        public PdlExpression Expression { get; private set; }

        public PdlExpressionAlteration(
            PdlTerm term,
            PdlExpression expression)
            : base(term)
        {
            Expression = expression;
            _hashCode = ComputeHashCode();
        }

        public override PdlNodeType NodeType => PdlNodeType.PdlExpressionAlteration;

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (!(obj is PdlExpressionAlteration expression))
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

        public override int GetHashCode() => _hashCode;

        public override string ToString() => $"{Term} | {Expression}";
    }
}