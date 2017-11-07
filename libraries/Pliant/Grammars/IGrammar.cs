using System.Collections.Generic;

namespace Pliant.Grammars
{
    public interface IGrammar
    {
        IReadOnlyList<IProduction> Productions { get; }

        INonTerminal Start { get; }

        IReadOnlyList<ILexerRule> Ignores { get; }

        IReadOnlyList<ILexerRule> Trivia { get; }

        IReadOnlyDottedRuleRegistry DottedRules { get; }

        IReadOnlyList<ILexerRule> LexerRules { get; }

        int GetLexerRuleIndex(ILexerRule lexerRule);

        IReadOnlyList<IProduction> RulesFor(INonTerminal nonTerminal);

        IReadOnlyList<IProduction> RulesContainingSymbol(INonTerminal nonTerminal);

        IReadOnlyList<IProduction> StartProductions();

        bool IsNullable(INonTerminal nonTerminal);

        bool IsTransativeNullable(INonTerminal nonTerminal);

        bool IsRightRecursive(ISymbol symbol);
    }
}