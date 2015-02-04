using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Earley
{
    public class NonTerminal : Symbol
    {
        public NonTerminal(string value)
            : base(SymbolType.NonTerminal, value)
        { }
    }
}
