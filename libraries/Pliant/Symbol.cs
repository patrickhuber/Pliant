using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Earley
{
    public abstract class Symbol : ISymbol
    {
        public SymbolType SymbolType { get; private set; }
        
        protected Symbol(SymbolType symbolType)
        {
            SymbolType = symbolType;
        }
    }
}
