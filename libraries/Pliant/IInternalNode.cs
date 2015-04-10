using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant
{
    public interface IInternalNode : INode
    {
        bool IsEmpty { get; }
        void AddChild(INode node);
        IReadOnlyList<INode> Children { get; }
    }
}
