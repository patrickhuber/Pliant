using System;

namespace Pliant.Ebnf
{
    public class EbnfExpression : EbnfNode
    {
        public EbnfTerm Term { get; private set; }

        public EbnfExpression(EbnfTerm term)
        {
            Term = term;
        }

        public override EbnfNodeType NodeType
        {
            get
            {
                return EbnfNodeType.EbnfExpression;
            }
        }
    }
    
    public class EbnfExpressionAlteration : EbnfExpression
    {
        public EbnfExpression Expression { get; private set; }

        public EbnfExpressionAlteration(
            EbnfTerm term,
            EbnfExpression expression)
            : base(term)
        {
        }

        public override EbnfNodeType NodeType
        {
            get
            {
                return EbnfNodeType.EbnfExpressionAlteration;
            }
        }
    }
}