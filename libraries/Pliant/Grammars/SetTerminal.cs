using Pliant.Utilities;
using System;
using System.Collections.Generic;

namespace Pliant.Grammars
{
    public class SetTerminal : BaseTerminal
    {
        private readonly HashSet<char> _characterSet;
        private readonly Interval[] _intervals;

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
            _intervals = CreateIntervals(characterSet);            
        }

        private static Interval[] CreateIntervals(ISet<char> characterSet)
        {
            var sortedListPool = SharedPools.Default<List<char>>();
            var sortedArray = sortedListPool.AllocateAndClear();
            sortedArray.AddRange(characterSet);
            sortedArray.Sort();

            var intervalListPool = SharedPools.Default<List<Interval>>();
            var intervalList = intervalListPool.AllocateAndClear();
            Interval? accumulator = null;

            for (var i = 0; i < sortedArray.Count; i++)
            {
                var interval = new Interval(sortedArray[i], sortedArray[i]);
                if (accumulator == null)
                {
                    accumulator = interval;
                    continue;
                }

                var joins = Interval.Join(accumulator.Value, interval);

                // two items mean that the intervals do not intersect
                // add the first interval to the list 
                // and set the accumulator to the second interval
                if (joins.Count == 2)
                {
                    intervalList.Add(joins[0]);
                    accumulator = joins[1];
                }
                else if (joins.Count == 1)
                {
                    accumulator = joins[0];
                }
            }

            sortedListPool.ClearAndFree(sortedArray);

            if (accumulator != null)
                intervalList.Add(accumulator.Value);

            var array = intervalList.ToArray();
            intervalListPool.ClearAndFree(intervalList);

            return array;
        }

        public override bool IsMatch(char character)
        {
            return _characterSet.Contains(character);
        }

        public override string ToString()
        {
            return $"[{string.Join(string.Empty, _characterSet)}]";
        }

        public override Interval[] GetIntervals()
        {
            return _intervals;
        }
    }
}