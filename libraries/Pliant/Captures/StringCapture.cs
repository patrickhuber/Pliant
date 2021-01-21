using System;
using System.Text;

namespace Pliant.Captures
{
    public class StringCapture : ICapture<char>
    {
        private readonly string _string;

        public StringCapture(string value)
        {
            _string = value;
        }

        public char this[int index] => _string[index];

        public int Count
        {
            get => _string.Length;
            set => throw new NotImplementedException();
        }

        public int Offset
        {
            get => 0;
            set => throw new NotImplementedException();
        }

        public ICapture<char> Parent => null;

        public bool IsReadOnly => true;

        public ICapture<char> Slice(int index)
        {
            return new Capture<char>(this, index, _string.Length - index);
        }

        public ICapture<char> Slice(int index, int count)
        {
            return new Capture<char>(this, index, count);
        }

        public override string ToString()
        {
            return _string;
        }

        public string ToString(int offset, int count)
        {
            return _string.Substring(offset, count);
        }

        public override int GetHashCode()
        {
            return _string is null
                ? 0
                : _string.GetHashCode() ^ Offset ^ Count;
        }

        public static bool operator ==(StringCapture left, ICapture<char> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(StringCapture left, ICapture<char> right)
        {
            return !(left == right);
        }

        public static bool operator ==(StringCapture left, string right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(StringCapture left, string right)
        {
            return !(left == right);
        }

        public static bool operator ==(StringCapture left, StringBuilder right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(StringCapture left, StringBuilder right)
        {
            return !(left == right);
        }

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
    }
}
