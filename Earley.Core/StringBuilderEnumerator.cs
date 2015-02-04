using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Earley
{
    public class StringBuilderEnumerator : IEnumerator<char>
    {
        private int _index;
        private StringBuilder _stringBuilder;
        public StringBuilderEnumerator(StringBuilder stringBuilder)
        {
            Assert.IsNotNull(stringBuilder, "stringBuilder");
            _stringBuilder = stringBuilder;
            _index = -1;
        }

        public char Current
        {
            get { return _stringBuilder[_index]; }
        }

        public void Dispose()
        {
        }

        object System.Collections.IEnumerator.Current
        {
            get { return _stringBuilder[_index]; }
        }

        public bool MoveNext()
        {
            if (_index < _stringBuilder.Length - 1)
            {
                _index++;
                return true;
            }
            return false;
        }

        public void Reset()
        {
            _index = -1;
        }
    }
}
