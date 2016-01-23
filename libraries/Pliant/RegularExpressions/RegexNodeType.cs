namespace Pliant.RegularExpressions
{
    public enum RegexNodeType
    {
        Regex,
        RegexAtom,
        RegexAtomAny,
        RegexAtomCharacter,
        RegexAtomExpression,
        RegexAtomSet,
        RegexCharacter,
        RegexCharacterClass,
        RegexCharacterClassList,
        RegexCharacterClassCharacter,
        RegexCharacterRange,
        RegexCharacterRangeSet,
        RegexExpression,
        RegexExpressionTerm,
        RegexExpressionAlteration,
        RegexFactor,
        RegexFactorIterator,
        RegexSet,
        RegexTerm,
        RegexTermFactor
    }
}