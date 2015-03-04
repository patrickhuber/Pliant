using System;
using System.Collections.Generic;
namespace Pliant
{
    public interface IGrammar
    {
        IReadOnlyList<IProduction> Productions { get; }
        
        IReadOnlyList<ILexeme> Lexemes { get; }
        
        INonTerminal Start { get; }
        
        IReadOnlyList<INonTerminal> Ignores { get; }

        IEnumerable<IProduction> RulesFor(INonTerminal nonTerminal);

        IEnumerable<ILexeme> LexemesFor(INonTerminal nonTerminal);

        IEnumerable<IProduction> StartProductions();
    }
}
