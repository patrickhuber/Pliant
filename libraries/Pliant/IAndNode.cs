using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant
{
    public interface IAndNode
    {
        IReadOnlyList<INode> Children { get; } 
    }
}
