using System.Collections.Generic;

namespace Pliant.Forest
{
    public interface IPackedForestNode : IForestNodeVisitable
    {
        IReadOnlyList<IForestNode> Children { get; }
    }
}