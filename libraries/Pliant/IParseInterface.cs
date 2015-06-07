using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant
{
    public interface IParseInterface
    {
        bool Read();
        int Position { get; }
        IParseEngine ParseEngine { get; }
    }
}
