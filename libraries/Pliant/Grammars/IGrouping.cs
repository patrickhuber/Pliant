using System.Collections.Generic;

namespace Pliant.Grammars
{
    public interface IGrouping : ISymbol
    {
        IReadOnlyCollection<ISymbol> Items { get; }
    }
}