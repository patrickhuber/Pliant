using System;
using System.Collections.Generic;
namespace Pliant
{
    public interface IGrammar
    {
        IReadOnlyList<IProduction> Productions { get; }
                
        INonTerminal Start { get; }
        
        IReadOnlyList<INonTerminal> Ignores { get; }

        IEnumerable<IProduction> RulesFor(INonTerminal nonTerminal);

        IEnumerable<IProduction> LexemesFor(INonTerminal nonTerminal);

        IEnumerable<IProduction> StartProductions();
    }
}
