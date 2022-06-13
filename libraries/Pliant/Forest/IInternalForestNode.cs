using System.Collections.Generic;

namespace Pliant.Forest
{
    /// <summary>
    /// Represents a Disjuncion of IPackedNodes
    /// </summary>
    public interface IInternalForestNode : IForestNode
    {
        IReadOnlyList<IPackedForestNode> Children { get; }

        void AddUniqueFamily(IForestNode trigger);

        void AddUniqueFamily(IForestNode source, IForestNode trigger);
    }
}