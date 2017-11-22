using System;
using System.Collections.Generic;

namespace Pliant.Grammars
{
    public class CharacterClassTerminal : BaseTerminal
    {
        private readonly List<ITerminal> _terminals;
        private IReadOnlyList<Interval> _intervals;

        public CharacterClassTerminal(params ITerminal[] terminals)
        {
            _terminals = new List<ITerminal>(terminals);
        }

        private static IReadOnlyList<Interval> CreateIntervals(IReadOnlyList<ITerminal> terminals)
        {
            var intervalList = new List<Interval>();
            for (var i = 0; i < terminals.Count; i++)
            {
                var intervals = terminals[i].GetIntervals();
                intervalList.AddRange(intervals);
            }
            return Interval.Group(intervalList);
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

        public override IReadOnlyList<Interval> GetIntervals()
        {
            if(_intervals == null)
                _intervals = CreateIntervals(_terminals);
            return _intervals;
        }
    }
}