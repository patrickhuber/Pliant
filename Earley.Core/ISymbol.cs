using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Earley
{
    public interface ISymbol
    {
        SymbolType SymbolType { get; }
    }
}
