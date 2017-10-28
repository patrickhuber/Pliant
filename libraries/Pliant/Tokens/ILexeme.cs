using Pliant.Grammars;
using Pliant.Tokens;

namespace Pliant.Tokens
{
    public interface ILexeme : IToken, ITrivia
    {
        bool Scan(char c);

        bool IsAccepted();
             
        ILexerRule LexerRule { get; }

        void AddTrailingTrivia(ITrivia trivia);

        void AddLeadingTrivia(ITrivia trivia);

        void Reset();
    }
}