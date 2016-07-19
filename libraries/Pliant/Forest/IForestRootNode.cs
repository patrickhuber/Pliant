using System.Collections.Generic;

namespace Pliant.Forest
{
    public interface IForestRootNode
    {
        IReadOnlyList<IAndForestNode> Children { get; }
    }
}