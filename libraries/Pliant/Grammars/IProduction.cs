using System.Collections.Generic;

namespace Pliant.Grammars
{
    public interface IProduction
    {
        INonTerminal LeftHandSide { get; }

        IReadOnlyList<ISymbol> RightHandSide { get; }

        bool IsEmpty { get; }
    }
}