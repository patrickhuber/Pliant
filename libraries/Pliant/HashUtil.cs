namespace Pliant
{
    internal static class HashUtil
    {
        private const uint SEED = 2166136261;
        private const int INCREMENTAL = 16777619;

        public static int ComputeHash(params int[] hashCodes)
        {
            unchecked
            {
                int hash = (int)SEED;
                for (int i = 0; i < hashCodes.Length; i++)
                {
                    hash = hash * INCREMENTAL ^ hashCodes[i];
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