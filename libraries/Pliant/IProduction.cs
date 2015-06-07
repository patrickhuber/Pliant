using System.Collections.Generic;

namespace Pliant
{
    public interface IProduction
    {
        INonTerminal LeftHandSide { get; }

        IReadOnlyList<ISymbol> RightHandSide { get; }

        bool IsEmpty { get; }
    }
}
