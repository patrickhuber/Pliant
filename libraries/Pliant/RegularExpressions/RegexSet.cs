namespace Pliant.RegularExpressions
{
    public class RegexSet
    {
        public bool Negate { get; private set; }

        public RegexCharacterClass CharacterClass {get; private set;}
        
        public RegexSet(bool negate, RegexCharacterClass characterClass)
        {
            Negate = negate;
            CharacterClass = characterClass;
        }

        public override bool Equals(object obj)
        {
            if ((object)obj == null)
                return false;

            var set = obj as RegexSet;
            if ((object)set == null)
                return false;

            return CharacterClass.Equals(set.CharacterClass)
                && Negate.Equals(set.Negate);
        }

        private bool _isHashCodeSet = false;
        private int _hashCode = 0;

        public override int GetHashCode()
        {
            if (!_isHashCodeSet)
            {
                _hashCode = HashUtil.ComputeHash(
                    Negate.GetHashCode(),
                    CharacterClass.GetHashCode());
                _isHashCodeSet = true;
            }
            return _hashCode;
        }

    }
}