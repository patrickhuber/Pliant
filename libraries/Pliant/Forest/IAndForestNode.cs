using System.Collections.Generic;

namespace Pliant.Forest
{
    public interface IAndForestNode
    {
        IReadOnlyList<IForestNode> Children { get; }
    }
}