using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant
{
    public interface IToken
    {
        StringBuilder Value { get; }
        int Origin { get; }
        int Type { get; }
    }
}
