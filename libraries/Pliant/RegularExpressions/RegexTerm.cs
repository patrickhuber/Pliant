namespace Pliant.RegularExpressions
{
    public class RegexTerm
    {
        public RegexFactor Factor { get; private set; }
        
        public RegexTerm(RegexFactor factor)
        {
            Factor = factor;
        }

        public override bool Equals(object obj)
        {
            if ((object)obj == null)
                return false;
            var term = obj as RegexTerm;
            if ((object)term == null)
                return false;
            return term.Factor.Equals(Factor);
        }

        private bool _isHashCodeSet = false;
        private int _hashCode = 0;

        public override int GetHashCode()
        {
            if (!_isHashCodeSet)
            {
                _hashCode = HashUtil.ComputeHash(Factor.GetHashCode());
                _isHashCodeSet = true;
            }
            return _hashCode;
        }
    }

    public class RegexTermFactor : RegexTerm
    {
        public RegexTerm Term { get; private set; }

        public RegexTermFactor(RegexFactor factor, RegexTerm term)
            : base(factor)
        {
            Term = term;
        }
        
        public override bool Equals(object obj)
        {
            if ((object)obj == null)
                return false;
            var termFactor = obj as RegexTermFactor;
            if ((object)termFactor == null)
                return false;
            return termFactor.Factor.Equals(Factor) 
                && termFactor.Term.Equals(Term);
        }

        private bool _isHashCodeSet = false;
        private int _hashCode = 0;

        public override int GetHashCode()
        {
            if (!_isHashCodeSet)
            {
                _hashCode = HashUtil.ComputeHash(
                    Term.GetHashCode(),
                    Factor.GetHashCode());
                _isHashCodeSet = true;
            }
            return _hashCode;
        }

    }
}