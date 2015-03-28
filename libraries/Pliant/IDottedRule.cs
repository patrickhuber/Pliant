using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant
{
    public interface IDottedRule
    {
        int Position { get; }
        ISymbol Symbol { get; }
        bool IsComplete { get; }
        IDottedRule NextRule();
    }
}
