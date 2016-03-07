using System.Collections;
using System.Collections.Generic;

namespace Pliant.Collections
{
    public interface IQueue<T> : IEnumerable<T>, ICollection, IEnumerable
    {
        void Enqueue(T item);
        T Dequeue();
        T Peek();
    }
}
