using System.Collections.Generic;

namespace Pliant.Grammars
{
    public class CharacterClassTerminal : BaseTerminal
    {
        private readonly List<ITerminal> _terminals;
        
        public CharacterClassTerminal(params ITerminal[] terminals)
        {
            _terminals = new List<ITerminal>(terminals);
        }

        public override bool IsMatch(char character)
        {
            // PERF: Avoid LINQ Any due to Lambda allocation
            foreach (var terminal in _terminals)
                if (terminal.IsMatch(character))
                    return true;
            return false;
        }
    }
}