using System;
using System.Text;

namespace Pliant.Captures
{
    public class StringBuilderCapture : ICapture<char>
    {
        private StringBuilder _stringBuilder;

        public StringBuilderCapture(StringBuilder stringBuilder)
        {
            _stringBuilder = stringBuilder;
        }

        public char this[int index] => _stringBuilder[index];

        public int Count
        {
            get => _stringBuilder.Length;
            set => throw new NotImplementedException($"{nameof(StringBuilderCapture)} is read only");
        }

        public int Offset
        {
            get => 0;
            set => throw new NotImplementedException($"{nameof(StringBuilderCapture)} is read only");
        }

        public ICapture<char> Parent => null;

        public ICapture<char> Slice(int index)
        {
            return new Capture<char>(this, index, _stringBuilder.Length - index);
        }

        public ICapture<char> Slice(int index, int count)
        {
            return new Capture<char>(this, index, count);
        }

        public bool IsReadOnly => true;

        public override bool Equals(object obj)
        {
            return obj switch
            {
                ICapture<char> capture => Equals(capture),
                StringBuilder builder => Equals(builder),
                string s => Equals(s),
                _ => false
            };
        }


        public bool Equals(ICapture<char> obj)
        {
            if (obj.Count != Count)
                return false;

            for (var i = 0; i < Count; i++)
                if (!this[i].Equals(obj[i]))
                    return false;

            return true;
        }

        public bool Equals(StringBuilder builder)
        {
            if (builder.Length != Count)
                return false;
            for (var i = 0; i < Count; i++)
                if (!this[i].Equals(builder[i]))
                    return false;
            return true;
        }

        public bool Equals(string s)
        {
            if (s.Length != Count)
                return false;
            for (var i = 0; i < Count; i++)
                if (!this[i].Equals(s[i]))
                    return false;
            return true;
        }

        public override int GetHashCode()
        {
            return null == _stringBuilder
                ? 0
                : _stringBuilder.GetHashCode() ^ Offset ^ Count;
        }

        public static bool operator ==(StringBuilderCapture left, ICapture<char> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(StringBuilderCapture left, ICapture<char> right)
        {
            return !(left == right);
        }

        public static bool operator ==(StringBuilderCapture left, string right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(StringBuilderCapture left, string right)
        {
            return !(left == right);
        }

        public static bool operator ==(StringBuilderCapture left, StringBuilder right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(StringBuilderCapture left, StringBuilder right)
        {
            return !(left == right);
        }

        public override string ToString()
        {
            return _stringBuilder.ToString();
        }

        public string ToString(int offset, int count)
        {
            return _stringBuilder.ToString(offset, count);
        }
    }
}
