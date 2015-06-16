using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Pliant.Collections
{
    public class ReadOnlyList<T> : ReadOnlyCollection<T>,  IReadOnlyList<T>
    {
        public ReadOnlyList(IList<T> list)
            : base(list)
        {            
        }
    }
}
