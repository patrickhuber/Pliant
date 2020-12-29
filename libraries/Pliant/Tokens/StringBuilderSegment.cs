using System;
using System.Text;

namespace Pliant.Tokens
{
    public struct StringBuilderSegment : ISegment<char>
    {
        private StringBuilder _stringBuilder;

        public StringBuilderSegment(StringBuilder stringBuilder)
        {
            _stringBuilder = stringBuilder;
        }

        public char this[int index] => _stringBuilder[index];

        public int Count 
        {
            get => _stringBuilder.Length;
            set =>  throw new NotImplementedException("StringBuilderSegment is read only"); 
        }

        public int Offset
        {
            get => 0;
            set => throw new NotImplementedException("StringBuilderSegment is read only");
        }

        public ISegment<char> Parent => null;

        public ISegment<char> Slice(int index)
        {
            return new Segment<char>(this, index, _stringBuilder.Length - index);
        }

        public ISegment<char> Slice(int index, int count)
        {
            return new Segment<char>(this, index, count);
        }

        public bool IsReadOnly => true;

        public override bool Equals(object obj)
        {
            return obj is StringBuilderSegment segment && Equals(segment);
        }

        public bool Equals(StringBuilderSegment obj)
        {
            return obj._stringBuilder.Equals(_stringBuilder) && obj.Offset == Offset && obj.Count == Count;
        }

        public override int GetHashCode()
        {
            return null == _stringBuilder
                ? 0
                : _stringBuilder.GetHashCode() ^ Offset ^ Count;
        }

        public static bool operator ==(StringBuilderSegment left, StringBuilderSegment right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(StringBuilderSegment left, StringBuilderSegment right)
        {
            return !(left == right);
        }
    }
}
