using System;
using System.Collections;
using System.Collections.Generic;

namespace Pliant.Collections
{
    public class UniqueList<T> : IList<T>, IReadOnlyList<T>
    {
        private HashSet<int> _index;
        private readonly List<T> _innerList;

        private const int Threshold = 10;

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
            if (!_index.Add(item.GetHashCode()))
                return false;

            _innerList.Insert(index, item);
            return false;
        }

        private bool InsertUniqueUsingList(int index, T item)
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
                _index.Remove(item.GetHashCode());
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
            if (!_index.Add(item.GetHashCode()))
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
            if (_index == null)
                _index = new HashSet<int>();

            if (_index.Count == _innerList.Count)
                return;

            for (int i = 0; i < _innerList.Count; i++)
                _index.Add(_innerList[i].GetHashCode());
        }

        public void Clear()
        {
            _innerList.Clear();
            if(_index != null)
                _index.Clear();
        }

        public bool ContainsHash(int hashcode)
        {
            if (HashSetIsMoreEfficient())
                return _index.Contains(hashcode);
            for (var i = 0; i < _innerList.Count; i++)
            {
                var item = _innerList[i];
                if (item.GetHashCode() == hashcode)
                    return true;
            }
            return false;
        }

        public bool Contains(T item)
        {
            if (HashSetIsMoreEfficient())
                return _index.Contains(item.GetHashCode());
            return _innerList.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _innerList.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            if (HashSetIsMoreEfficient())
            {
                _index.Remove(item.GetHashCode());
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

        public override int GetHashCode()
        {
            return _innerList.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (((object)obj) == null)
                return false;
            var uniqueList = obj as UniqueList<T>;
            if (((object)uniqueList) == null)
                return false;
            return _innerList.Equals(uniqueList._innerList);
        }

        public T[] ToArray()
        {
            return _innerList.ToArray();
        }
    }
}
