using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Earley
{
    public interface INonTerminal : ISymbol
    {
        string Value { get; }
    }
}
