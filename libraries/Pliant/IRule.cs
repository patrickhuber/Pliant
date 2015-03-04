using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant
{
    public interface IRule<TSymbol> 
        where TSymbol : ISymbol
    {
        INonTerminal LeftHandSide { get; }
        IReadOnlyList<TSymbol> RightHandSide { get; }
    }
}
