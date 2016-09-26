using System;
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
            for (int t = 0; t < _terminals.Count; t++)
            {
                var terminal = _terminals[t];
                if (terminal.IsMatch(character))
                    return true;
            }
            return false;
        }

        public override Interval[] GetIntervals()
        {
            throw new NotImplementedException();
        }
    }
}