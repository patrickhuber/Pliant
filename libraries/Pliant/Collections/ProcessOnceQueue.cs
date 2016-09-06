using System;
using System.Collections;
using System.Collections.Generic;

namespace Pliant.Collections
{
    public class ProcessOnceQueue<T> : IQueue<T>
    {
        private readonly Queue<T> _queue;
        private readonly Dictionary<T,T> _visited;

        public ProcessOnceQueue()
        {
            _queue = new Queue<T>();
            _visited = new Dictionary<T, T>();
        }

        public ProcessOnceQueue(IEnumerable<T> items)
            : this()
        {
            foreach (var item in items)
                Enqueue(item);
        }

        public void Clear()
        {
            _queue.Clear();
            _visited.Clear();
        }

        public void Enqueue(T item)
        {
            if (_visited.ContainsKey(item))
                return;

            _visited[item] = item;
            _queue.Enqueue(item);            
        }
        
        public T EnqueueOrGetExisting(T item)
        {
            if (!_visited.ContainsKey(item))
            {
                _visited[item] = item;
                _queue.Enqueue(item);
                return item;
            }
            return _visited[item];
        }

        public T Dequeue()
        {
            return _queue.Dequeue();
        }

        public T Peek()
        {
            return _queue.Peek();
        }

        public T[] ToArray()
        {
            return _queue.ToArray();
        }

        public void TrimExcess()
        {
            _queue.TrimExcess();
        }

        public IEnumerable<T> Visited { get { return _visited.Keys; } }

        public IEnumerator<T> GetEnumerator()
        {
            return _queue.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _queue.GetEnumerator();
        }

        public void CopyTo(Array array, int index)
        {
            ((ICollection)_queue).CopyTo(array, index);
        }

        public int Count { get { return _queue.Count; } }

        public object SyncRoot
        {
            get
            {
                return ((ICollection)_queue).SyncRoot;
            }
        }

        public bool IsSynchronized
        {
            get
            {
                return ((ICollection)_queue).IsSynchronized;
            }
        }
    }
}
