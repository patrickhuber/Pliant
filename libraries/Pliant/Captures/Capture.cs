using System;
using System.Collections.Generic;
using System.Text;

namespace Pliant.Captures
{
    public class Capture<T> : ICapture<T>
    {
        private const string ArrayIndexNonNegative = "an array index must be a non negative number";
        private int _offset;
        private int _count;

        public Capture(ICapture<T> parent, int offset, int count)
        {
            if (parent == null)
                throw new ArgumentNullException("parent");
            if (parent.Count - offset < count)
                throw new ArgumentException("offset and length must be within the bounds of the original segment");

            Offset = offset;
            Count = count;
            Parent = parent;
        }

        public T this[int index] => Parent[Offset + index];

        public int Count
        {
            get => _count;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), ArrayIndexNonNegative);
                _count = value;
            }
        }

        public int Offset
        {
            get => _offset;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(value), ArrayIndexNonNegative);
                _offset = value;
            }
        }

        public ICapture<T> Parent { get; private set; }

        public ICapture<T> Slice(int index)
        {
            return new Capture<T>(Parent, Offset + index, Count - index);
        }

        public ICapture<T> Slice(int index, int count)
        {
            return new Capture<T>(Parent, Offset + index, count);
        }

        public bool IsReadOnly => false;

        public override bool Equals(object obj)
        {
            return obj is ICapture<T> segment && Equals(segment);
        }

        public bool Equals(ICapture<T> obj)
        {
            if (obj.Count != Count)
                return false;
            for (var i = 0; i < Count; i++)
                if (!EqualityComparer<T>.Default.Equals(this[i], obj[i]))
                    return false;
            return true;
        }

        public override int GetHashCode()
        {
            return null == Parent
                ? 0
                : Parent.GetHashCode() ^ Offset ^ Count;
        }

        public static bool operator ==(Capture<T> left, ICapture<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Capture<T> left, ICapture<T> right)
        {
            return !(left == right);
        }

        public override string ToString()
        {
            return Parent.ToString(Offset, Count);
        }

        public string ToString(int offset, int count)
        {
            return Parent.ToString(offset, count);
        }
    }
}
