using System;

namespace Pliant.Ebnf
{
    public class EbnfRule : EbnfNode
    {
        private readonly int _hashCode;

        public EbnfQualifiedIdentifier QualifiedIdentifier { get; private set; }
        public EbnfExpression Expression { get; private set; }

        public EbnfRule(EbnfQualifiedIdentifier qualifiedIdentifier, EbnfExpression expression)
        {
            QualifiedIdentifier = qualifiedIdentifier;
            Expression = expression;
            _hashCode = ComputeHashCode();
        }

        public override EbnfNodeType NodeType
        {
            get
            {
                return EbnfNodeType.EbnfRule;
            }
        }

        int ComputeHashCode()
        {
            return HashUtil.ComputeHash(
                QualifiedIdentifier.GetHashCode(),
                Expression.GetHashCode(),
                NodeType.GetHashCode());
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override bool Equals(object obj)
        {
            if ((object)obj == null)
                return false;
            var rule = obj as EbnfRule;
            if ((object)rule == null)
                return false;
            return rule.NodeType == EbnfNodeType.EbnfRule
                && rule.QualifiedIdentifier.Equals(QualifiedIdentifier)
                && rule.Expression.Equals(Expression);
        }
    }
}