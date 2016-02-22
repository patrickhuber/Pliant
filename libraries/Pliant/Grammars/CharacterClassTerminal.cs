using System.Collections.Generic;

namespace Pliant.Grammars
{
    public class CharacterClassTerminal : ITerminal
    {
        private readonly IList<ITerminal> _terminals;

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