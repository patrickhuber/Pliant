using System;

namespace Pliant.Ebnf
{
    public class EbnfTerm : EbnfNode
    {
        public EbnfFactor Factor { get; private set; }
        
        public EbnfTerm(EbnfFactor factor)
        {
            Factor = factor;
        }

        public override EbnfNodeType NodeType
        {
            get
            {
                return EbnfNodeType.EbnfTerm;
            }
        }
    }

    public class EbnfTermRepetition : EbnfTerm
    {
        public EbnfTerm Term { get; private set; }

        public EbnfTermRepetition(EbnfFactor factor, EbnfTerm term)
            : base(factor)
        {
            Term = term;
        }

        public override EbnfNodeType NodeType
        {
            get
            {
                return EbnfNodeType.EbnfTermRepetition;
            }
        }
    }
}