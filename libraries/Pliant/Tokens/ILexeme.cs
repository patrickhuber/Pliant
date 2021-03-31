using Pliant.Grammars;

namespace Pliant.Tokens
{
    public interface ILexeme : IToken, ITrivia
    {
        /// <summary>
        /// Scan the current character from the underlying Segment
        /// </summary>
        /// <returns></returns>
        bool Scan();

        /// <summary>
        /// Returns true if the Lexeme is in an accepted state, false otherwise.
        /// </summary>
        /// <returns></returns>
        bool IsAccepted();
             
        /// <summary>
        /// The underlying lexer rule used to run this lexeme
        /// </summary>
        ILexerRule LexerRule { get; }

        void AddTrailingTrivia(ITrivia trivia);

        void AddLeadingTrivia(ITrivia trivia);

        void Reset();
    }
}