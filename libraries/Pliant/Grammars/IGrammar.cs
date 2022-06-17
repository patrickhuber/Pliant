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

        /// <summary>
        /// Checks if a symbol is directly nullable.
        /// </summary>
        /// <param name="nonTerminal"></param>
        /// <returns></returns>
        bool IsNullable(INonTerminal nonTerminal);

        /// <summary>
        /// Checks if a symbol is nullable through transative nullablitity. There is a path of derivation where the symbol can be replaced with null.
        /// </summary>
        /// <param name="nonTerminal"></param>
        /// <returns></returns>
        bool IsTransativeNullable(INonTerminal nonTerminal);

        bool IsRightRecursive(ISymbol symbol);
    }
}