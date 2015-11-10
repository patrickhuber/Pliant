namespace Pliant.RegularExpressions
{
    public class RegexAtom
    {
    }

    public class RegexAtomAny : RegexAtom
    { }

    public class RegexAtomCharacter : RegexAtom
    {
        public RegexCharacter Character { get; set; }
    }

    public class RegexAtomExpression : RegexAtom
    {
        public RegexExpression Expression { get; set; }
    }

    public class RegexAtomSet : RegexAtom
    {
        public RegexSet Set { get; set; }
    }

    
}