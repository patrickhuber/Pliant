using System.Collections.Generic;

namespace Pliant.Grammars
{
    public interface IGrammar
    {
        IReadOnlyList<IProduction> Productions { get; }

        INonTerminal Start { get; }

        IReadOnlyList<ILexerRule> Ignores { get; }

        IReadOnlyList<IProduction> RulesFor(INonTerminal nonTerminal);

        IEnumerable<IProduction> StartProductions();

        bool IsNullable(INonTerminal nonTerminal);
    }
}