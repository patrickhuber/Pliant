using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pliant.Grammars
{
    public interface ISymbol
    {
        SymbolType SymbolType { get; }
    }
}
