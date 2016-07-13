using System.Collections.Generic;

namespace Pliant.Utilities
{
    internal class ObjectPool<T> where T : class, new()
    {
        private readonly Queue<T> _queue;
        private readonly ObjectPoolFactory _factory;

        internal delegate T ObjectPoolFactory();

        internal ObjectPool()
            : this(()=>new T())
        { }

        internal ObjectPool(int size)
            : this(size, ()=>new T())
        { }

        internal ObjectPool(int size, ObjectPoolFactory factory)
        {
            _factory = factory;
            _queue = new Queue<T>(size);
        }

        internal ObjectPool(ObjectPoolFactory factory)
            : this(20, factory)
        {
        }
        
        internal T Allocate()
        {
            if (_queue.Count == 0)
                return CreateInstance();
            return _queue.Dequeue();
        }

        private T CreateInstance()
        {
            return _factory();
        }

        internal void Free(T value)
        {
            _queue.Enqueue(value);
        }
        
    }
}
