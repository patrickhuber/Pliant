using Pliant.Utilities;

namespace Pliant.Languages.Regex
{
    public class RegexSet : RegexNode
    {
        public bool Negate { get; private set; }

        public RegexCharacterClass CharacterClass { get; private set; }

        public override RegexNodeType NodeType
        {
            get { return RegexNodeType.RegexSet; }
        }

        public RegexSet(bool negate, RegexCharacterClass characterClass)
        {
            Negate = negate;
            CharacterClass = characterClass;
            _hashCode = ComputeHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;

            if (!(obj is RegexSet set))
                return false;

            return CharacterClass.Equals(set.CharacterClass)
                && Negate.Equals(set.Negate);
        }
        
        private readonly int _hashCode ;

        int ComputeHashCode()
        {
            return HashCode.Compute(
                Negate.GetHashCode(),
                CharacterClass.GetHashCode());
        }
        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override string ToString()
        {
            return $"[{(Negate ? "^" : string.Empty)}{CharacterClass}]";
        }
    }
}