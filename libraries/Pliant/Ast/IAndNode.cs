using System.Collections.Generic;

namespace Pliant.Ast
{
    public interface IAndNode
    {
        IReadOnlyList<INode> Children { get; }
    }
}