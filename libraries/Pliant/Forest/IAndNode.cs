using System.Collections.Generic;

namespace Pliant.Forest
{
    public interface IAndNode
    {
        IReadOnlyList<INode> Children { get; }
    }
}