namespace Pliant.Grammars
{
    public interface IGrammarLexerRule : ILexerRule
    {
        IGrammar Grammar { get; }
    }
}