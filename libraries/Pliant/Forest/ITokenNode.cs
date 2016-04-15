using Pliant.Tokens;

namespace Pliant.Forest
{
    public interface ITokenNode : INode
    {
        IToken Token { get; }
    }
}