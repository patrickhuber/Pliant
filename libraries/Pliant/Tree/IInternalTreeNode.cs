using Pliant.Grammars;
using System.Collections.Generic;

namespace Pliant.Tree
{
    public interface IInternalTreeNode : ITreeNode
    {
        INonTerminal Symbol { get; }
        IEnumerable<ITreeNode> Children { get; }
    }
}