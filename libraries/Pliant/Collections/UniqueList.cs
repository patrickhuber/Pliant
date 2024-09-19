using System.Collections;
using System.Collections.Generic;

namespace Pliant.Collections
{
    public class UniqueList<T> : IList<T>, IReadOnlyList<T>
    {
        private HashSet<int> _index;
        private readonly List<T> _innerList;

        public int Count { get { return _innerList.Count; } }

        public bool IsReadOnly { get { return false; } }

        public T this[int index]
        {
            get { return _innerList[index]; }
            set { _innerList[index] = value; }
        }

        public UniqueList()
        {
            _innerList = new List<T>();
            _index = new HashSet<int>();
        }

        public UniqueList(int capacity)
        {
            _innerList = new List<T>(capacity);
            _index = new HashSet<int>();
        }

        public UniqueList(IEnumerable<T> list)
        {
            _innerList = new List<T>(list);
            for(int i = 0; i < _innerList.Count; i++)
            {
                var item = _innerList[i];
                _index.Add(item.GetHashCode());
            }
        }

        public int IndexOf(T item)
        {
            return _innerList.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            InsertUnique(index, item);
        }

        public bool InsertUnique(int index, T item)
        {
            if (!_index.Add(item.GetHashCode()))
                return false;
            
            _innerList.Insert(index, item);
            return true;
        }

        public void RemoveAt(int index)
        {
            var item = _innerList[index];
            _index.Remove(item.GetHashCode());
            _innerList.RemoveAt(index);
        }

        public void Add(T item)
        {
            AddUnique(item);
        }

        public bool AddUnique(T item)
        {
            if (!_index.Add(item.GetHashCode()))
                return false;

            _innerList.Add(item);
            return true;
        }

        public void Clear()
        {
            _innerList.Clear();
            _index.Clear();
        }

        public bool ContainsHash(int hashcode)
        {
            return _index.Contains(hashcode);
        }

        public bool Contains(T item)
        {            
            return _index.Contains(item.GetHashCode());
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _innerList.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            _index.Remove(item.GetHashCode());            
            return _innerList.Remove(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _innerList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _innerList.GetEnumerator();
        }

        public override int GetHashCode()
        {
            return _innerList.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (!(obj is UniqueList<T> uniqueList))
                return false;
            return _innerList.Equals(uniqueList._innerList);
        }

        public T[] ToArray()
        {
            return _innerList.ToArray();
        }
    }
}
