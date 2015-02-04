using System;
using System.Collections.Generic;
namespace Earley
{
    public interface IGrammar
    {
        IReadOnlyList<IProduction> Productions { get; }
        IEnumerable<IProduction> RulesFor(ISymbol symbol);
    }
}
