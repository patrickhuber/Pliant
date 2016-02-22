using System;

namespace Pliant.Ebnf
{
    public class EbnfTerm : EbnfNode
    {
        private readonly Lazy<int> _hashCode;

        public EbnfFactor Factor { get; private set; }
        
        public EbnfTerm(EbnfFactor factor)
        {
            Factor = factor;
            _hashCode = new Lazy<int>(ComputeHashCode);
        }

        public override EbnfNodeType NodeType
        {
            get
            {
                return EbnfNodeType.EbnfTerm;
            }
        }

        int ComputeHashCode()
        {
            return HashUtil.ComputeHash(
                NodeType.GetHashCode(),
                Factor.GetHashCode());
        }

        public override int GetHashCode()
        {
            return _hashCode.Value;
        }

        public override bool Equals(object obj)
        {
            if ((object)obj == null)
                return false;
            var term = obj as EbnfTerm;
            if ((object)term == null)
                return false;
            return term.NodeType == NodeType
                && term.Factor.Equals(Factor);
        }
    }

    public class EbnfTermRepetition : EbnfTerm
    {
        private readonly Lazy<int> _hashCode;

        public EbnfTerm Term { get; private set; }

        public EbnfTermRepetition(EbnfFactor factor, EbnfTerm term)
            : base(factor)
        {
            Term = term;
            _hashCode = new Lazy<int>(ComputeHashCode);
        }

        public override EbnfNodeType NodeType
        {
            get
            {
                return EbnfNodeType.EbnfTermRepetition;
            }
        }

        int ComputeHashCode()
        {
            return HashUtil.ComputeHash(
                NodeType.GetHashCode(),
                Factor.GetHashCode(),
                Term.GetHashCode());
        }

        public override int GetHashCode()
        {
            return _hashCode.Value;
        }

        public override bool Equals(object obj)
        {
            if ((object)obj == null)
                return false;
            var term = obj as EbnfTermRepetition;
            if ((object)term == null)
                return false;
            return term.NodeType == NodeType
                && term.Factor.Equals(Factor)
                && term.Term.Equals(Term);
        }
    }
}