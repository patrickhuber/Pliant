using System;
using System.Collections.Generic;
namespace Earley
{
    public interface IProduction
    {
        INonTerminal LeftHandSide { get; }
        IReadOnlyList<ISymbol> RightHandSide { get; }
    }
}
