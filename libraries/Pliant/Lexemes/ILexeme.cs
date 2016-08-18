using Pliant.Grammars;
using Pliant.Tokens;

namespace Pliant.Lexemes
{
    public interface ILexeme
    {
        string Capture { get; }

        bool Scan(char c);

        bool IsAccepted();

        TokenType TokenType { get; }

        ILexerRule LexerRule { get; }
    }
}