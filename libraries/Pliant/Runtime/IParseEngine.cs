using Pliant.Forest;
using Pliant.Charts;
using Pliant.Grammars;
using Pliant.Tokens;
using System.Collections.Generic;
using Pliant.Utilities;

namespace Pliant.Runtime
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
        IInternalForestNode GetParseForestRootNode();

        /// <summary>
        /// Returns the list of expected lexer rules based on the current state of the parse.
        /// </summary>
        /// <returns>The enumeration of lexer rules that apply at the current parse position.</returns>
        IReadOnlyList<ILexerRule> GetExpectedLexerRules();
        
        /// <summary>
        /// Accepts one token and incrementally moves the parser forward if successful.
        /// </summary>
        /// <param name="token">The next token to move the parser forward.</param>
        /// <returns>True if the token was accepted, false otherwise.</returns>
        bool Pulse(IToken token);

        /// <summary>
        /// Accepts multiple tokens into the current Earley Set and incrementally moves the parser forward if any are successful
        /// </summary>
        /// <param name="tokens"></param>
        /// <returns></returns>
        bool Pulse(IReadOnlyList<IToken> tokens);

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