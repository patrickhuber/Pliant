using Pliant.Collections;
using System.Collections.Generic;

namespace Pliant
{
    public interface IGrammar
    {
        IReadOnlyList<IProduction> Productions { get; }
                
        INonTerminal Start { get; }
        
        IReadOnlyList<ILexerRule> Ignores { get; }

        IEnumerable<IProduction> RulesFor(INonTerminal nonTerminal);

        IEnumerable<ILexerRule> LexerRulesFor(INonTerminal nonTerminal);

        IEnumerable<IProduction> StartProductions();
    }
}
