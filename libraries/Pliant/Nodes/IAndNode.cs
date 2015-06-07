using System.Collections.Generic;

namespace Pliant.Nodes
{
    public interface IAndNode
    {
        IReadOnlyList<INode> Children { get; } 
    }
}
