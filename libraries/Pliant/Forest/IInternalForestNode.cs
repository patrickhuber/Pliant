using System.Collections.Generic;

namespace Pliant.Forest
{
    /// <summary>
    /// Represents a Disjuncion of IAndNodes
    /// </summary>
    public interface IInternalForestNode : IForestNode
    {
        IReadOnlyList<IAndForestNode> Children { get; }

        void AddUniqueFamily(IForestNode trigger);

        void AddUniqueFamily(IForestNode source, IForestNode trigger);
    }
}