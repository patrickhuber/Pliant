using Pliant.Utilities;
using System;

namespace Pliant.Ebnf
{
    public class EbnfLexerRule : EbnfNode
    {
        public EbnfQualifiedIdentifier QualifiedIdentifier { get; private set; }
        public EbnfLexerRuleExpression Expression { get; private set; }

        private readonly int _hashCode;

        public override EbnfNodeType NodeType
        {
            get
            {
                return EbnfNodeType.EbnfLexerRule;
            }
        }

        public EbnfLexerRule(EbnfQualifiedIdentifier qualifiedIdentifier, EbnfLexerRuleExpression expression)
        {
            QualifiedIdentifier = qualifiedIdentifier;
            Expression = expression;
            _hashCode = ComputeHashCode();
        }

        public override bool Equals(object obj)
        {
            if ((object)obj == null)
                return false;
            var ebnfLexerRule = obj as EbnfLexerRule;
            if ((object)ebnfLexerRule == null)
                return false;

            return ebnfLexerRule.QualifiedIdentifier.Equals(QualifiedIdentifier)
                && ebnfLexerRule.Expression.Equals(Expression);
        }

        private int ComputeHashCode()
        {
            return HashCode.Compute(
                NodeType.GetHashCode(),
                QualifiedIdentifier.GetHashCode(),
                Expression.GetHashCode());
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }
    }
}