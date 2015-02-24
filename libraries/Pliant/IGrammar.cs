using System;
using System.Collections.Generic;
namespace Pliant
{
    public interface IGrammar
    {
        IReadOnlyList<IProduction> Productions { get; }
        IEnumerable<IProduction> RulesFor(INonTerminal nonTerminal);
    }
}
