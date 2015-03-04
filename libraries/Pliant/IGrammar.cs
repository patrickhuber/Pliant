using System;
using System.Collections.Generic;
namespace Pliant
{
    public interface IGrammar
    {
        IReadOnlyList<IProduction> Productions { get; }
        
        IReadOnlyList<ILexeme> Lexemes { get; }
        
        INonTerminal Start { get; }
        
        IReadOnlyList<INonTerminal> Ignore { get; }

        IEnumerable<IProduction> RulesFor(INonTerminal nonTerminal);

        IEnumerable<ILexeme> LexemeFor(INonTerminal nonTerminal);

        IEnumerable<IProduction> StartProductions();
    }
}
