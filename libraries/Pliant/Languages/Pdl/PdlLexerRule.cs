using Pliant.Utilities;

namespace Pliant.Languages.Pdl
{

    public class PdlLexerRule : PdlNode
    {
        public PdlQualifiedIdentifier QualifiedIdentifier { get; private set; }
        public PdlLexerRuleExpression Expression { get; private set; }

        private readonly int _hashCode;

        public override PdlNodeType NodeType => PdlNodeType.PdlLexerRule;

        public PdlLexerRule(PdlQualifiedIdentifier qualifiedIdentifier, PdlLexerRuleExpression expression)
        {
            QualifiedIdentifier = qualifiedIdentifier;
            Expression = expression;
            _hashCode = ComputeHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (!(obj is PdlLexerRule ebnfLexerRule))
                return false;

            return ebnfLexerRule.QualifiedIdentifier.Equals(QualifiedIdentifier)
                && ebnfLexerRule.Expression.Equals(Expression);
        }

        private int ComputeHashCode()
        {
            return HashCode.Compute(
                ((int)NodeType).GetHashCode(),
                QualifiedIdentifier.GetHashCode(),
                Expression.GetHashCode());
        }

        public override int GetHashCode() => _hashCode;
    }
}