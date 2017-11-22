using System.Collections.Generic;

namespace Pliant.Tokens
{
    public interface IToken
    {
        string Value { get; }
        int Position { get; }
        TokenType TokenType { get; }
        IReadOnlyList<ITrivia> LeadingTrivia { get; }
        IReadOnlyList<ITrivia> TrailingTrivia { get; }
    }
}