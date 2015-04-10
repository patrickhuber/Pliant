using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant
{
    public interface ISymbolNode : IInternalNode
    {
        ISymbol Symbol { get;}
    }
}
