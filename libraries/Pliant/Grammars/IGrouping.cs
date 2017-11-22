using System.Collections.Generic;

namespace Pliant.Grammars
{
    public interface IGrouping : ISymbol
    {
        IReadOnlyList<ISymbol> Items { get; }
    }
}