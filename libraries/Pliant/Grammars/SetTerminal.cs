using Pliant.Utilities;
using System;
using System.Collections.Generic;

namespace Pliant.Grammars
{
    public class SetTerminal : BaseTerminal
    {
        private readonly HashSet<char> _characterSet;
        private IReadOnlyList<Interval> _intervals;

        public SetTerminal(params char[] characters)
            : this(new HashSet<char>(characters))
        {
        }

        public SetTerminal(char first)
        {
            _characterSet = new HashSet<char>();
            _characterSet.Add(first);
        }

        public SetTerminal(char first, char second)
            : this(first)
        {
            _characterSet.Add(second);
        }

        public SetTerminal(ISet<char> characterSet)
        {
            _characterSet = new HashSet<char>(characterSet);
            _intervals = CreateIntervals(_characterSet);            
        }
                

        private static IReadOnlyList<Interval> CreateIntervals(HashSet<char> characterSet)
        {
            var intervalListPool = SharedPools.Default<List<Interval>>();
            var intervalList = intervalListPool.AllocateAndClear();

            // create a initial set of intervals
            foreach (var character in characterSet)            
                intervalList.Add(new Interval(character, character));

            var groupedIntervals = Interval.Group(intervalList);
            intervalListPool.ClearAndFree(intervalList);

            return groupedIntervals;
        }

        public override bool IsMatch(char character)
        {
            return _characterSet.Contains(character);
        }

        public override string ToString()
        {
            return $"[{string.Join(string.Empty, _characterSet)}]";
        }

        public override IReadOnlyList<Interval> GetIntervals()
        {
            if(_intervals == null)
                _intervals = CreateIntervals(_characterSet); 
            return _intervals;
        }
    }
}