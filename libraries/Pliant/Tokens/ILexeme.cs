using Pliant.Grammars;
using Pliant.Tokens;

namespace Pliant.Tokens
{
    public interface ILexContext
    {
        void ReadCharacter(int position, char character);
    }

    public interface ILexeme : IToken
    {
        bool Scan(ILexContext context, char c);

        bool IsAccepted();
             
        ILexerRule LexerRule { get; }
    }
}