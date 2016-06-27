using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant.Utilities
{
    public static class ObjectPoolExtensions
    {
        internal static List<T> AllocateAndClear<T>(this ObjectPool<List<T>> pool)
        {
            var list = pool.Allocate();
            list.Clear();
            return list;
        }        
    }
}
