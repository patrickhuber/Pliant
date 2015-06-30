using System.Collections.Generic;
using System.Linq;

namespace Pliant.Collections
{
    public static class IEnumerableExtensions
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null)
                return true;
            // PERF: Removing Linq Any for some reason increased performance
            foreach (var item in enumerable)
                return false;
            return true;
        }
    }
}
