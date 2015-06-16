using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant.Collections
{
    public class ReadWriteList<T> : List<T>, IList<T>, IReadOnlyList<T>
    {        
    }
}
