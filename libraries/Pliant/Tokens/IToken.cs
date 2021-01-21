using Pliant.Captures;
using Pliant.Grammars;
using System.Collections.Generic;

namespace Pliant.Tokens
{
    public interface IToken
    {
        //string Value { get; }
        ICapture<char> Capture { get; }
        int Position { get; }
        TokenType TokenType { get; }
        IReadOnlyList<ITrivia> LeadingTrivia { get; }
        IReadOnlyList<ITrivia> TrailingTrivia { get; }
    }
}