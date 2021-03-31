using System.Collections.Generic;

namespace Pliant.Grammars
{
    public interface IRepetition : ISymbol
    {
        IReadOnlyList<ISymbol> Items { get; }
    }
}
