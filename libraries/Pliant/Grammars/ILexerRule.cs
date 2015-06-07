using Pliant.Tokens;

namespace Pliant.Grammars
{
    public interface ILexerRule : ISymbol
    {
        IGrammar Grammar { get; }
        TokenType TokenType { get; }
    }
}
