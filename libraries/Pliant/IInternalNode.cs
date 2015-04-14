using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant
{
    /// <summary>
    /// Represents a Disjuncion of IAndNodes
    /// </summary>
    public interface IInternalNode : INode
    {
        bool IsEmpty { get; }
        IReadOnlyList<IAndNode> Children { get; }
        void AddUniqueFamily(INode trigger);
        void AddUniqueFamily(INode source, INode trigger);
    }
}
