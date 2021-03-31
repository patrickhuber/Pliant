using System.Collections.Generic;

namespace Pliant.Grammars
{
    public interface IOptional : ISymbol
    {
        IReadOnlyList<ISymbol> Items { get; }
    }
}
