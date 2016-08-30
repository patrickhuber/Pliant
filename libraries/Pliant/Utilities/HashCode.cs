using System.Collections.Generic;

namespace Pliant.Utilities
{
    internal static class HashCode
    {
        private const uint SEED = 2166136261;
        private const int INCREMENTAL = 16777619;
        
        public static int Compute(int first)
        {
            unchecked
            {
                var hash = (int)SEED;
                hash = hash * INCREMENTAL ^ first;
                return hash;
            }
        }

        public static int Compute(int first, int second)
        {
            unchecked
            {
                var hash = (int)SEED;
                hash = hash * INCREMENTAL ^ first;
                hash = hash * INCREMENTAL ^ second;
                return hash;
            }
        }

        public static int Compute(int first, int second, int third)
        {
            unchecked
            {
                var hash = (int)SEED;
                hash = hash * INCREMENTAL ^ first;
                hash = hash * INCREMENTAL ^ second;
                hash = hash * INCREMENTAL ^ third;
                return hash;
            }
        }

        public static int Compute(int first, int second, int third, int fourth)
        {
            unchecked
            {
                var hash = (int)SEED;
                hash = hash * INCREMENTAL ^ first;
                hash = hash * INCREMENTAL ^ second;
                hash = hash * INCREMENTAL ^ third;
                hash = hash * INCREMENTAL ^ fourth;
                return hash;
            }
        }

        public static int Compute(int first, int second, int third, int fourth, int fifth)
        {
            unchecked
            {
                var hash = (int)SEED;
                hash = hash * INCREMENTAL ^ first;
                hash = hash * INCREMENTAL ^ second;
                hash = hash * INCREMENTAL ^ third;
                hash = hash * INCREMENTAL ^ fourth;
                hash = hash * INCREMENTAL ^ fifth;
                return hash;
            }
        }

        public static int Compute(int first, int second, int third, int fourth, int fifth, int sixth)
        {
            unchecked
            {
                var hash = (int)SEED;
                hash = hash * INCREMENTAL ^ first;
                hash = hash * INCREMENTAL ^ second;
                hash = hash * INCREMENTAL ^ third;
                hash = hash * INCREMENTAL ^ fourth;
                hash = hash * INCREMENTAL ^ fifth;
                hash = hash * INCREMENTAL ^ sixth;
                return hash;
            }
        }

        public static int Compute(IEnumerable<object> items)
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