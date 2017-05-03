using System;
using System.Collections;
using System.Collections.Generic;

namespace Pliant.Collections
{
    public class IndexedList<T> : IList<T>, IReadOnlyList<T>
    {
        private Dictionary<T, int> _indexOfDictionary;
        private List<T> _innerList;

        public int Count
        {
            get { return _innerList.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public T this[int index]
        {
            get { return _innerList[index]; }
            set
            {
                Insert(index, value);
            }
        }

        public void Add(T item)
        {
            if (_indexOfDictionary.ContainsKey(item))
                return;
            _indexOfDictionary[item] = _innerList.Count;
            _innerList.Add(item);
        }

        public void Clear()
        {
            _indexOfDictionary.Clear();
            _innerList.Clear();
        }

        public bool Contains(T item)
        {
            return _indexOfDictionary.ContainsKey(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _innerList.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _innerList.GetEnumerator();
        }

        public int IndexOf(T item)
        {
            int value;
            if (!_indexOfDictionary.TryGetValue(item, out value))
                return -1;
            return value;
        }

        public IndexedList()
        {
            _indexOfDictionary = new Dictionary<T, int>();
            _innerList = new List<T>();
        }

        public IndexedList(IEnumerable<T> enumerable)
            : this()
        {
            if (enumerable is IReadOnlyList<T>)
            {
                var list = enumerable as IReadOnlyList<T>;
                for (var i = 0; i < list.Count; i++)
                {
                    Add(list[i]);
                }
            }
            else
            {
                foreach (var item in enumerable)
                {
                    Add(item);
                }
            }
        }

        public void Insert(int index, T item)
        {
            var oldIndex = IndexOf(item);
            if (oldIndex >= 0)
            {
                _innerList.RemoveAt(oldIndex);
                if (oldIndex < index)
                    index -= 1;
            }
            _innerList.Insert(index, item);
            ShiftLeft(Math.Min(index, oldIndex >= 0 ? oldIndex : index));
        }

        public bool Remove(T item)
        {
            var indexOf = IndexOf(item);
            if (indexOf < 0)
                return false;
            if (!_indexOfDictionary.Remove(item))
                return false;
            _innerList.RemoveAt(indexOf);
            ShiftLeft(indexOf);
            return true;
        }

        public void RemoveAt(int index)
        {
            var item = _innerList[index];
            _innerList.RemoveAt(index);
            _indexOfDictionary.Remove(item);
            ShiftLeft(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _innerList.GetEnumerator();
        }

        private void ShiftLeft(int index)
        {
            for (var i = index; i < _innerList.Count; i++)
                _indexOfDictionary[_innerList[i]] = i;
        }
    }
}
