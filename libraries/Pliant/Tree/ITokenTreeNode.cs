using Pliant.Tokens;

namespace Pliant.Tree
{
    public interface ITokenTreeNode : ITreeNode
    {
        IToken Token { get; }
    }
}