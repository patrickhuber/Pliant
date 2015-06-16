using System.Collections.Generic;

namespace Pliant.Nodes
{
    /// <summary>
    /// Represents a Disjuncion of IAndNodes
    /// </summary>
    public interface IInternalNode : INode
    {
        IReadOnlyList<IAndNode> Children { get; }
        void AddUniqueFamily(INode trigger);
        void AddUniqueFamily(INode source, INode trigger);
    }
}
