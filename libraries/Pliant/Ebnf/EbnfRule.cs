using System;

namespace Pliant.Ebnf
{
    public class EbnfRule : EbnfNode
    {
        public EbnfQualifiedIdentifier QualifiedIdentifier { get; private set; }
        public EbnfExpression Expression { get; private set; }

        public EbnfRule(EbnfQualifiedIdentifier qualifiedIdentifier, EbnfExpression expression)
        {
            QualifiedIdentifier = qualifiedIdentifier;
            Expression = expression;
        }

        public override EbnfNodeType NodeType
        {
            get
            {
                return EbnfNodeType.EbnfRule;
            }
        }
    }
}