using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant.Utilities
{
    internal static class SharedPools
    {
        public static ObjectPool<T> Default<T>() where T : class, new()
        {
            return DefaultPool<T>.Instance;
        }

        private static class DefaultPool<T> where T : class, new()
        {
            public static readonly ObjectPool<T> Instance = new ObjectPool<T>(20, () => new T());
        }
    }
}
