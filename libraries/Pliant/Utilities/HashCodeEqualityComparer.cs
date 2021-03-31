using System.Collections.Generic;

namespace Pliant.Utilities
{
    class HashCodeEqualityComparer<T> : IEqualityComparer<T>
    {
        public bool Equals(T x, T y)
        {
            return x.GetHashCode().Equals(y.GetHashCode());
        }

        public int GetHashCode(T obj)
        {
            return obj.GetHashCode();
        }
    }
}
