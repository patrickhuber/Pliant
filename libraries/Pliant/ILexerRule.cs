using Pliant.Grammars;
using Pliant.Tokens;

namespace Pliant
{
    public interface ILexerRule : ISymbol
    {
        IGrammar Grammar { get; }
        TokenType TokenType { get; }
    }
}
