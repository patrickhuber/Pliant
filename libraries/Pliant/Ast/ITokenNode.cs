using Pliant.Tokens;

namespace Pliant.Ast
{
    public interface ITokenNode : INode
    {
        IToken Token { get; }
    }
}