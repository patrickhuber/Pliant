using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant
{
    public class MetaTerminal : ITerminal
    {
        public bool IsMatch(char character)
        {
            throw new NotImplementedException();
        }

        public SymbolType SymbolType { get { return SymbolType.Terminal; } }
    }
}
