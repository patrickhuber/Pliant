namespace Pliant.Grammars
{
    public interface IStringLiteralLexerRule : ILexerRule
    {
        string Literal { get; }
    }
}