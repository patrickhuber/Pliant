using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant
{
    public interface INullable<T>
    {
        T Value { get; }
        bool HasValue { get; }
    }
}
