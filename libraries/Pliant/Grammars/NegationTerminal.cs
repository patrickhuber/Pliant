using System;
using System.Collections.Generic;

namespace Pliant.Grammars
{
    public class NegationTerminal : BaseTerminal
    {
        public ITerminal InnerTerminal { get; private set; }

        private readonly IReadOnlyList<Interval> _intervals;

        public NegationTerminal(ITerminal innerTerminal)
        {
            InnerTerminal = innerTerminal;
            var inverseIntervalList = new List<Interval>();
            var intervals = innerTerminal.GetIntervals();
            for (var i = 0; i < intervals.Count; i++)
            {
                var inverseIntervals = Interval.Inverse(intervals[i]);
                inverseIntervalList.AddRange(inverseIntervals);
            }
            
            _intervals = Interval.Group(inverseIntervalList);
        }

        public override bool IsMatch(char character)
        {
            return !InnerTerminal.IsMatch(character);
        }

        public override IReadOnlyList<Interval> GetIntervals()
        {
            return _intervals;
        }
    }
}