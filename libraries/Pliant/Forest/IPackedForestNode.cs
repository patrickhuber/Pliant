using System.Collections.Generic;

namespace Pliant.Forest
{
    public interface IPackedForestNode
    {
        IReadOnlyList<IForestNode> Children { get; }
    }
}