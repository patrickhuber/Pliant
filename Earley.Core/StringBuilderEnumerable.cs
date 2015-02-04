using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Earley
{
    public class StringBuilderEnumerable : IEnumerable<char>
    {
        private StringBuilder _stringBuilder;
        
        public StringBuilderEnumerable(StringBuilder stringBuilder)
        {
            _stringBuilder = stringBuilder;
        }

        public IEnumerator<char> GetEnumerator()
        {
            return new StringBuilderEnumerator(_stringBuilder);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new StringBuilderEnumerator(_stringBuilder);
        }
    }
}
