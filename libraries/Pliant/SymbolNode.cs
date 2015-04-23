using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant
{
    public class SymbolNode : InternalNode, ISymbolNode
    {
        public ISymbol Symbol { get; private set; }
        
        public override NodeType NodeType { get { return NodeType.Symbol; } }

        public SymbolNode(ISymbol symbol, int origin, int location)
            : base(origin, location)
        {            
            Symbol = symbol;
        }

        public override string ToString()
        {
            return string.Format("({0}, {1}, {2})", Symbol, Origin, Location);
        }
    }
}
