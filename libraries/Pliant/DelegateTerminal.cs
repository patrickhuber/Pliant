using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Earley
{
    public class DelegateTerminal : Symbol, ITerminal
    {
        private Func<char, bool> _isMatch;

        public DelegateTerminal(Func<char, bool> isMatch)
            : base(SymbolType.Terminal)
        {
            _isMatch = isMatch;
        }

        public bool IsMatch(char character)
        {
            return _isMatch(character);
        }
    }
}
