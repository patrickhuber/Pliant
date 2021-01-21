using Pliant.Captures;
using Pliant.Grammars;

namespace Pliant.Tokens
{
    public interface ITrivia
    {
        int Position { get; }
        ICapture<char> Capture { get; }
        TokenType TokenType { get; }
    }
}
