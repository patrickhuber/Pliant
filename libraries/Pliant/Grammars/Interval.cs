using Pliant.Utilities;
using System;
using System.Collections.Generic;

namespace Pliant.Grammars
{
    /// <summary>
    /// Defines an interval with a inclusive min and max. Does not represent empty sets. Represents single value sets with Min == Max
    /// </summary> 
    public struct Interval : IComparable<Interval>
    {
        /// <summary>
        /// Gets the min value of the interval
        /// </summary>
        public readonly char Min;

        /// <summary>
        /// Gets the max value of the interval
        /// </summary>
        public readonly char Max;
        
        /// <summary>
        /// Constructs a new interval with the give min and max values
        /// </summary>
        /// <param name="min">the min inclusive value of the interval</param>
        /// <param name="max">the max inclusive value of the interval</param>
        public Interval(char min, char max)
        {
            if (min.CompareTo(max) > 0)
                throw new Exception($"{min} should be less than {max}");

            Min = min;
            Max = max;
        }
        
        /// <summary>
        /// Compares two intervals
        /// </summary>
        /// <param name="other">the interval being compared</param>
        /// <returns>if mins are not equal returns CompareTo value of each min. If they are equal returns CompareTo of each max.</returns>
        public int CompareTo(Interval other)
        {
            var compareMin = Min.CompareTo(other.Min);
            if (compareMin != 0)
                return compareMin;
            return Max.CompareTo(other.Max);
        }

        /// <summary>
        /// Determines when two ranges overlap or intersect
        /// </summary>
        /// <param name="other">the interval being compared</param>
        /// <returns>true when the intervals overlap. false otherwise</returns>
        public bool Overlaps(Interval other)
        {
            return Max.CompareTo(other.Min) >= 0 && Min.CompareTo(other.Max) <= 0;
        }

        /// <summary>
        /// Determines when two intervals touch. Touch is when one Min differes from a Max by at most one.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Touches(Interval other)
        {
            // if intervals overlap they touch
            if (Overlaps(other))
                return true;

            // char.CompareTo returns the difference between integers
            if (Min - other.Max == 1)
                return true;

            if (other.Min - Max == 1)
                return true;

            return false;
        }

        /// <summary>
        /// Joins two intervals together if they overlap.
        /// </summary>
        /// <param name="first">the first interval</param>
        /// <param name="second">the second interval</param>
        /// <returns>
        /// if intervals overlap returns one interval representing the global min and global max of both intervals.
        /// if intervals are equal returns a single interval.
        /// if intervals do not overlap reutrns both interval.
        /// </returns>
        public static IReadOnlyList<Interval> Join(Interval first, Interval second)
        {
            var list = new List<Interval>();

            var touches = first.Touches(second);
            if (!touches)
            {
                list.Add(first);
                list.Add(second);
                return list;
            }

            var max = (char)Math.Max(first.Max, second.Max);
            var min = (char)Math.Min(first.Min, second.Min);

            var interval = new Interval(min, max);
            list.Add(interval);

            return list;
        }

        /// <summary>
        /// if the intervals do not overlap, they are echoed back.
        /// if the intervals overlap one interval is created that covers the overlap.
        /// an additional interval is created for the minimum values if they are not equal.
        /// an additional interval is created for the maximum values if they are not equal.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns>The disjoint set of intervals</returns>
        public static IReadOnlyList<Interval> Split(Interval first, Interval second)
        {
            var list = new List<Interval>();

            var overlaps = first.Overlaps(second);
            if (!overlaps)
            {
                list.Add(first);
                list.Add(second);
                return list;
            }

            // order matters to ensure intervals are sorted

            var minsAreEqual = first.Min.CompareTo(second.Min) == 0;
            var maxesAreEqual = first.Max.CompareTo(second.Max) == 0;
                        
            if (!minsAreEqual && maxesAreEqual)
            {
                var localMin = (char)Math.Min(first.Min, second.Min);
                var localMax = (char)(Math.Max(first.Min, second.Min) - 1);
                list.Add(new Interval(localMin, localMax));
            }

            var intersectMin = (char)Math.Max(first.Min, second.Min);
            var intersectMax = (char)Math.Min(first.Max, second.Max);
            list.Add(new Interval(intersectMin, intersectMax));
            
            if (minsAreEqual && !maxesAreEqual)
            {
                var localMin = (char)(Math.Min(first.Max, second.Max) + 1);
                var localMax = (char)Math.Max(first.Max, second.Max);
                list.Add(new Interval(localMin, localMax));
            }
            
            return list;
        }

        private static Interval[] EmptyList = { };

        public static IReadOnlyList<Interval> Inverse(Interval interval)
        {
            if (interval.Min == char.MinValue && interval.Max == char.MaxValue)
                return EmptyList;
            
            var list = new List<Interval>();
            if (interval.Min != char.MinValue)
                list.Add(new Interval(char.MinValue, (char)(interval.Min - 1)));
            if (interval.Max != char.MaxValue)
                list.Add(new Interval((char)(interval.Max + 1), char.MaxValue));

            return list;
        }

        public static IReadOnlyList<Interval> Group(IReadOnlyList<Interval> input)
        {
            var intervalPool = SharedPools.Default<List<Interval>>();
            var sortedIntervals = intervalPool.AllocateAndClear();
            sortedIntervals.AddRange(input);
            sortedIntervals.Sort();

            var intervalList = intervalPool.AllocateAndClear();

            Interval? accumulator = null;
            for (var i = 0; i < sortedIntervals.Count; i++)
            {
                var interval = sortedIntervals[i];
                if (accumulator == null)
                {
                    accumulator = interval;
                    continue;
                }

                var joins = Join(accumulator.Value, interval);

                switch (joins.Count)
                {
                    case 2:
                        intervalList.Add(joins[0]);
                        accumulator = joins[1];
                        break;
                    case 1:
                        accumulator = joins[0];
                        break;
                }
            }

            if (accumulator != null)
                intervalList.Add(accumulator.Value);

            intervalPool.ClearAndFree(sortedIntervals);

            return intervalList;
        }

        public override string ToString()
        {
            return $"'[{Min}', '{Max}']";
        }
    }
}
