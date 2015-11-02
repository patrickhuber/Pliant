using System.Collections.Generic;

namespace Pliant.Tree
{
    public interface IInternalTreeNode : ITreeNode
    {
        IEnumerable<ITreeNode> Children { get; }
    }
}
