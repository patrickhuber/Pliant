using System;
using System.Collections.Generic;
namespace Earley
{
    public interface IProduction
    {
        ISymbol LeftHandSide { get; }
        IReadOnlyList<ISymbol> RightHandSide { get; }
    }
}
