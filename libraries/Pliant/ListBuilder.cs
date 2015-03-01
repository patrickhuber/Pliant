using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant
{
    public class ListBuilder<T> : IListBuilder<T>
    {
        private IList<T> _list;
        public IListBuilder<T> Add(T item)
        {
            _list.Add(item);
            return this;
        }

        public IList<T> GetList() { return _list; }
    }
}
