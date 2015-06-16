using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant.Nodes
{
    public interface ITerminalNode : INode
    {
        char Capture { get; }
    }
}
