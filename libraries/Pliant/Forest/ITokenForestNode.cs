using Pliant.Tokens;

namespace Pliant.Forest
{
    public interface ITokenForestNode : IForestNode
    {
        IToken Token { get; }
    }
}