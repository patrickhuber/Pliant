using System;
using System.Collections.Generic;
using System.Text;

namespace Pliant.Tokens
{
    public class Segment<T> : ISegment<T>
    {
        private const string ArrayIndexNonNegative = "an array index must be a non negative number";
        private int _offset;
        private int _count;

        public Segment(ISegment<T> parent, int offset, int count)
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
                if(value < 0)
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

        public ISegment<T> Parent { get; private set; }

        public ISegment<T> Slice(int index)
        {
            return new Segment<T>(Parent, Offset + index, Count - index);
        }

        public ISegment<T> Slice(int index, int count)
        {            
            return new Segment<T>(Parent, Offset + index, count);
        }

        public bool IsReadOnly => false;

        public override bool Equals(object obj)
        {
            return obj is Segment<T> segment && Equals(segment);
        }

        public bool Equals(Segment<T> obj)
        {
            return obj.Parent == Parent && obj.Offset == Offset && obj.Count == Count;
        }

        public override int GetHashCode()
        {
            return null == Parent
                ? 0
                : Parent.GetHashCode() ^ Offset ^ Count;
        }

        public static bool operator ==(Segment<T> left, Segment<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Segment<T> left, Segment<T> right)
        {
            return !(left == right);
        }
    }
}
