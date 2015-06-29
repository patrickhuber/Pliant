using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant
{
    internal static class HashUtil
    {
        public static int ComputeHash(params int[] hashCodes)
        {
            unchecked
            {
                int hash = (int)2166136261;
                for (int i = 0; i < hashCodes.Length; i++)
                    hash = hash * 16777619 ^ hashCodes[i];
                return hash;
            }
        }
    }
}
