using Pliant.Grammars;
using System.Collections.Generic;

namespace Pliant.Tree
{
    public interface IInternalTreeNode : ITreeNode
    {
        INonTerminal Symbol { get; }
        IReadOnlyList<ITreeNode> Children { get; }
    }
}