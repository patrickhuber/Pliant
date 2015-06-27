using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant.Grammars
{
    public class CharacterClassTerminal : ITerminal
    {
        private IList<ITerminal> _terminals;

        public SymbolType SymbolType { get { return SymbolType.Terminal; } }

        public CharacterClassTerminal(params ITerminal[] terminals)
        {
            _terminals = terminals;
        }

        public bool IsMatch(char character)
        {
            // PERF: Avoid LINQ Any due to Lambda allocation
            foreach (var terminal in _terminals)
                if (terminal.IsMatch(character))
                    return true;
            return false;
        }
    }
}
