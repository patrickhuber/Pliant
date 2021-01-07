using Pliant.Utilities;
using System;

namespace Pliant.Languages.Pdl
{
    public class PdlTerm : PdlNode
    {
        private readonly int _hashCode;

        public PdlFactor Factor { get; private set; }
        
        public PdlTerm(PdlFactor factor)
        {
            Factor = factor;
            _hashCode = ComputeHashCode();
        }

        public override PdlNodeType NodeType
        {
            get
            {
                return PdlNodeType.PdlTerm;
            }
        }

        int ComputeHashCode()
        {
            return HashCode.Compute(
                ((int)NodeType).GetHashCode(),
                Factor.GetHashCode());
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (!(obj is PdlTerm term))
                return false;
            return term.NodeType == NodeType
                && term.Factor.Equals(Factor);
        }

        public override string ToString()
        {
            return Factor.ToString();
        }
    }

    public class PdlTermConcatenation : PdlTerm
    {
        private readonly int _hashCode;

        public PdlTerm Term { get; private set; }

        public PdlTermConcatenation(PdlFactor factor, PdlTerm term)
            : base(factor)
        {
            Term = term;
            _hashCode = ComputeHashCode();
        }

        public override PdlNodeType NodeType
        {
            get
            {
                return PdlNodeType.PdlTermConcatenation;
            }
        }

        int ComputeHashCode()
        {
            return HashCode.Compute(
                ((int)NodeType).GetHashCode(),
                Factor.GetHashCode(),
                Term.GetHashCode());
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (!(obj is PdlTermConcatenation term))
                return false;
            return term.NodeType == NodeType
                && term.Factor.Equals(Factor)
                && term.Term.Equals(Term);
        }

        public override string ToString()
        {
            return $"{Factor} {Term}";
        }
    }
}