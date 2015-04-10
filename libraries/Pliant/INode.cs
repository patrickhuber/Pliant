using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant
{
    public interface INode
    {
        int Origin { get; }
        int Location { get; }
    }
}
