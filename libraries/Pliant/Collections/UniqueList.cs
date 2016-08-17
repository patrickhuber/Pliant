using System;
using System.Collections;
using System.Collections.Generic;

namespace Pliant.Collections
{
    public class UniqueList<T> : IList<T>, IReadOnlyList<T>
    {
        private HashSet<T> _set;
        private readonly List<T> _innerList;

        private const int Threshold = 20;

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
        }

        public UniqueList(int capacity)
        {
            _innerList = new List<T>(capacity);
        }

        public UniqueList(IEnumerable<T> list)
        {
            _innerList = new List<T>(list);
            if (HashSetIsMoreEfficient())
                AllocateAndPopulateHashSet();
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
            if (HashSetIsMoreEfficient())
                return InsertUniqueUsingHashSet(index, item);

            return InsertUniqueUsingList(index, item);
        }

        private bool InsertUniqueUsingHashSet(int index, T item)
        {
            if (!_set.Add(item))
                return false;

            _innerList.Insert(index, item);
            return false;
        }

        public bool InsertUniqueUsingList(int index, T item)
        {
            if (_innerList.Count == 0)
            {
                _innerList.Insert(index, item);
                return true;
            }
            
            var hashCode = item.GetHashCode();
            for (int i = 0; i < _innerList.Count; i++)
            {
                var listItem = _innerList[i];
                if (hashCode.Equals(listItem.GetHashCode()))
                    return false;                
            }
            _innerList.Insert(index, item);
            if (HashSetIsMoreEfficient())
                AllocateAndPopulateHashSet();
            return true;
        }

        public void RemoveAt(int index)
        {
            if (HashSetIsMoreEfficient())
            {
                var item = _innerList[index];
                _set.Remove(item);
            }
            _innerList.RemoveAt(index);
        }

        public void Add(T item)
        {
            AddUnique(item);
        }

        public bool AddUnique(T item)
        {
            if (HashSetIsMoreEfficient())
                return AddUniqueUsingHashSet(item);
            return AddUniqueUsingList(item);
        }

        private bool AddUniqueUsingHashSet(T item)
        {
            if (!_set.Add(item))
                return false;
            _innerList.Add(item);
            return true;
        }

        private bool AddUniqueUsingList(T item)
        {
            if (_innerList.Count == 0)
            {                
                _innerList.Add(item);
                return true;
            }
            var hashCode = item.GetHashCode();
            for(int i=0;i<_innerList.Count;i++)
            {
                var listItem = _innerList[i];
                if (hashCode.Equals(listItem.GetHashCode()))
                    return false;
            }
            _innerList.Add(item);
            if (HashSetIsMoreEfficient())
                AllocateAndPopulateHashSet();
            return true;
        }

        private void AllocateAndPopulateHashSet()
        {
            if(_set == null)
                _set = new HashSet<T>();
            for (int i = 0; i < _innerList.Count; i++)
                _set.Add(_innerList[i]);
        }

        public void Clear()
        {
            _innerList.Clear();
            _set.Clear();
        }

        public bool Contains(T item)
        {
            if (HashSetIsMoreEfficient())
                return _set.Contains(item);
            return _innerList.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(T item)
        {
            if (HashSetIsMoreEfficient())
            {
                _set.Remove(item);
            }
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

        private bool HashSetIsMoreEfficient()
        {
            return _innerList.Count >= Threshold;
        }
    }
}
