using System;
using System.Collections.Generic;

namespace Pliant.Grammars
{
    public class NegationTerminal : BaseTerminal
    {
        public ITerminal InnerTerminal { get; private set; }

        private IReadOnlyList<Interval> _intervals;

        public NegationTerminal(ITerminal innerTerminal)
        {
            InnerTerminal = innerTerminal;           
        }

        private static IReadOnlyList<Interval> CreateIntervals(ITerminal innerTerminal)
        {
            var inverseIntervalList = new List<Interval>();
            var intervals = innerTerminal.GetIntervals();
            for (var i = 0; i < intervals.Count; i++)
            {
                var inverseIntervals = Interval.Inverse(intervals[i]);
                inverseIntervalList.AddRange(inverseIntervals);
            }

            return Interval.Group(inverseIntervalList);
        }

        public override bool IsMatch(char character)
        {
            return !InnerTerminal.IsMatch(character);
        }

        public override IReadOnlyList<Interval> GetIntervals()
        {
            if(_intervals == null)
                _intervals = CreateIntervals(InnerTerminal);
            return _intervals;
        }
    }
}