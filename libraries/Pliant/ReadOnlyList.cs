using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant
{
    public class ReadOnlyList<T> : ReadOnlyCollection<T>,  IReadOnlyList<T>
    {
        public ReadOnlyList(IList<T> list)
            : base(list)
        {            
        }
    }
}
