using Pliant.Utilities;
using System;
using System.Collections.Generic;

namespace Pliant.Grammars
{
    public class NegationTerminal : BaseTerminal
    {
        public ITerminal InnerTerminal { get; private set; }

        private IReadOnlyList<Interval> _intervals;

        private readonly int _hashCode = 0;

        public NegationTerminal(ITerminal innerTerminal)
        {
            InnerTerminal = innerTerminal;
            _hashCode = ComputeHashCode();
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
            if(_intervals is null)
                _intervals = CreateIntervals(InnerTerminal);
            return _intervals;
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;

            if (!(obj is NegationTerminal negationTerminal))
                return false;

            return negationTerminal.InnerTerminal.Equals(InnerTerminal);
        }

        private int ComputeHashCode()
        {
            return HashCode.Compute(InnerTerminal.GetHashCode(), "!".GetHashCode());
        }
    }
}