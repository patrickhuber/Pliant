using Pliant.Collections;
using System.Collections.Generic;

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

        internal static ReadWriteList<T> AllocateAndClear<T>(this ObjectPool<ReadWriteList<T>> pool)
        {
            var list = pool.Allocate();
            list.Clear();
            return list;
        }

        internal static Dictionary<TKey, TValue> AllocateAndClear<TKey, TValue>(this ObjectPool<Dictionary<TKey, TValue>> pool)
        {
            var dictionary = pool.Allocate();
            dictionary.Clear();
            return dictionary;
        }

        internal static void ClearAndFree<T>(this ObjectPool<List<T>> pool, List<T> list)
        {
            if (list == null)
                return;
            if (pool == null)
                return;
            list.Clear();
            pool.Free(list);
        } 
    }
}
