using System.Collections.Generic;

namespace Pliant
{
    public interface IAndNode
    {
        IReadOnlyList<INode> Children { get; } 
    }
}
