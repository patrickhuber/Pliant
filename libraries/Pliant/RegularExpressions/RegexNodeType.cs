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
        RegexCharacterClassAlteration,
        RegexCharacterClassCharacter,
        RegexCharacterRangeUnit,
        RegexCharacterRange,
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