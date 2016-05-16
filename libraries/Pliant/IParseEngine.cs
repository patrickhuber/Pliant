using Pliant.Forest;
using Pliant.Charts;
using Pliant.Grammars;
using Pliant.Tokens;
using System.Collections.Generic;

namespace Pliant
{
    /// <summary>
    /// Provides the interface for the earley parse engine.
    /// </summary>
    public interface IParseEngine
    {
        /// <summary>
        /// Resets the parse engine clearing out any current state.
        /// </summary>
        void Reset();

        /// <summary>
        /// Gets the current accepted status of the parse.
        /// </summary>
        /// <returns>true if the parse is in a accepted state / false otherwise. </returns>
        bool IsAccepted();

        /// <summary>
        /// Gets the root of the parse forest.
        /// </summary>
        /// <returns></returns>
        IForestNode GetParseForestRoot();

        /// <summary>
        /// Returns the list of expected lexer rules based on the current state of the parse.
        /// </summary>
        /// <returns>The enumeration of lexer rules that apply at the current parse position.</returns>
        IEnumerable<ILexerRule> GetExpectedLexerRules();

        /// <summary>
        /// Gets the current parse chart for use in recovery rules.
        /// </summary>
        IReadOnlyChart Chart { get; }

        /// <summary>
        /// Accepts one token and incrementally moves the parser forward if successful.
        /// </summary>
        /// <param name="token">The next token to move the parser forward.</param>
        /// <returns>True if the token was accepted, false otherwise.</returns>
        bool Pulse(IToken token);

        /// <summary>
        /// Gets the grammar used by the parse engine.
        /// </summary>
        IGrammar Grammar { get; }

        /// <summary>
        /// Gets the location in the parse. The location is the current token index and not the character index in the parse.
        /// </summary>
        int Location { get; }
    }
}