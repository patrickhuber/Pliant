namespace Pliant.RegularExpressions
{
    public class RegexFactor
    {
        public RegexAtom Atom { get; private set; }
        
        public RegexFactor(RegexAtom atom)
        {
            Atom = atom;
        }

        private bool _isHashCodeSet = false;
        private int _hashCode = 0;

        public override int GetHashCode()
        {
            if (!_isHashCodeSet)
            {
                _hashCode = HashUtil.ComputeHash(
                    Atom.GetHashCode());
                _isHashCodeSet = true;
            }
            return _hashCode;
        }

        public override bool Equals(object obj)
        {
            if ((object)obj == null)
                return false;
            var factor = obj as RegexFactor;
            if (factor == null)
                return false;
            return factor.Atom.Equals(Atom);
        }
    }

    public class RegexFactorIterator : RegexFactor
    {
        public RegexIterator Iterator { get; private set; }
        
        public RegexFactorIterator(RegexAtom atom, RegexIterator iterator)
            : base(atom)
        {
            Iterator = iterator;
        }

        public override bool Equals(object obj)
        {
            if ((object)obj == null)
                return false;
            var factor = obj as RegexFactor;
            if ((object)factor == null)
                return false;
            return factor.Atom.Equals(Atom);
        }

        private bool _isHashCodeSet = false;
        private int _hashCode = 0;

        public override int GetHashCode()
        {
            if (!_isHashCodeSet)
            {
                _hashCode = HashUtil.ComputeHash(
                    Atom.GetHashCode(),
                    Iterator.GetHashCode());
                _isHashCodeSet = true;
            }
            return _hashCode;
        }

    }
}