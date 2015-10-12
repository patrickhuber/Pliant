using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant.Grammars
{
    public abstract class BaseTerminal : ITerminal
    {
        public SymbolType SymbolType { get { return SymbolType.Terminal; } }
        
        public abstract bool IsMatch(char character);
    }
}
