using Pliant.Utilities;

namespace Pliant.Languages.Pdl
{
    public class PdlRule : PdlNode
    {
        private readonly int _hashCode;

        public PdlQualifiedIdentifier QualifiedIdentifier { get; private set; }
        public PdlExpression Expression { get; private set; }

        public PdlRule(PdlQualifiedIdentifier qualifiedIdentifier, PdlExpression expression)
        {
            QualifiedIdentifier = qualifiedIdentifier;
            Expression = expression;
            _hashCode = ComputeHashCode();
        }

        public override PdlNodeType NodeType { get => PdlNodeType.PdlRule; }

        int ComputeHashCode()
        {
            return HashCode.Compute(
                QualifiedIdentifier.GetHashCode(),
                Expression.GetHashCode(),
                NodeType.GetHashCode());
        }

        public override int GetHashCode() => _hashCode;

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (!(obj is PdlRule rule))
                return false;
            return rule.NodeType == PdlNodeType.PdlRule
                && rule.QualifiedIdentifier.Equals(QualifiedIdentifier)
                && rule.Expression.Equals(Expression);
        }

        public override string ToString() =>  $"{QualifiedIdentifier} = {Expression}";

    }
}