using System.Collections.Generic;

namespace Pliant
{
    internal static class HashUtil
    {
        private const uint SEED = 2166136261;
        private const int INCREMENTAL = 16777619;

        public static int ComputeHash(params int[] hashes)
        {
            unchecked
            {
                var hash = (int)SEED;
                foreach (var value in hashes)
                    hash = hash * INCREMENTAL ^ value;
                return hash;
            }
        }
        
        public static int ComputeHash(IEnumerable<object> items)
        {
            unchecked
            {
                var hash = (int)SEED;
                foreach (var item in items)
                {
                    hash = hash * INCREMENTAL ^ item.GetHashCode();
                }
                return hash;
            }
        }

        public static int ComputeIncrementalHash(int hashCode, int accumulator, bool isFirstValue = false)
        {
            unchecked
            {
                if (isFirstValue)
                {
                    accumulator = (int)SEED;
                }
                accumulator = accumulator * INCREMENTAL ^ hashCode;
                return accumulator;
            }
        }
    }
}