﻿using Pliant.Utilities;

namespace Pliant.Languages.Regex
{
    public class RegexTerm : RegexNode
    {
        public RegexFactor Factor { get; private set; }

        public override RegexNodeType NodeType
        {
            get { return RegexNodeType.RegexTerm; }
        }

        public RegexTerm(RegexFactor factor)
        {
            Factor = factor;
            _hashCode = ComputeHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (!(obj is RegexTerm term))
                return false;
            return term.Factor.Equals(Factor);
        }
        
        private readonly int _hashCode;
        
        int ComputeHashCode()
        {
            return HashCode.Compute(Factor.GetHashCode());
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override string ToString()
        {
            return Factor.ToString();
        }
    }

    public class RegexTermFactor : RegexTerm
    {
        public RegexTerm Term { get; private set; }

        public RegexTermFactor(RegexFactor factor, RegexTerm term)
            : base(factor)
        {
            Term = term;
            _hashCode = ComputeHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (!(obj is RegexTermFactor termFactor))
                return false;
            return termFactor.Factor.Equals(Factor)
                && termFactor.Term.Equals(Term);
        }
        
        private readonly int _hashCode;

        int ComputeHashCode()
        {
            return HashCode.Compute(
                    Term.GetHashCode(),
                    Factor.GetHashCode());
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override RegexNodeType NodeType
        {
            get { return RegexNodeType.RegexTermFactor; }
        }

        public override string ToString()
        {
            return $"{Factor}{Term}";
        }
    }
}