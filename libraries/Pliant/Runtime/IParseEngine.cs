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
        /// Initializes the parse engine for the specified context.
        /// </summary>
        /// <param name="context"></param>
        void Initialize(IParseContext context);
        
        /// <summary>
        /// Accepts one token and incrementally moves the parser forward if successful.
        /// </summary>
        /// <param name="context">The parse context to parse for</param>
        /// <param name="token">The token to move the parser forward with</param>
        /// <returns>True if the token was accepted, false otherwise.</returns>
        bool Pulse(IParseContext context, IToken token);

        /// <summary>
        /// Resets the parse engine clearing out any current state.
        /// </summary>
        void Reset(IParseContext context);

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
        List<ILexerRule> GetExpectedLexerRules();
        
        /// <summary>
        /// Gets the grammar used by the parse engine.
        /// </summary>
        IGrammar Grammar { get; }

        /// <summary>
        /// Gets the location in the parse. The location is the current token index and not the character index in the parse.
        /// </summary>
        int Location { get; }
    }


    /// <summary>
    /// Prediction modes for the parse engine
    /// </summary>
    public enum PredictionMode
    {
        Earley,
        AycockHorspool
    }

    /// <summary>
    /// Completion modes for the parse engine
    /// </summary>
    public enum CompletionMode
    {
        Earley,
        Leo
    }

    /// <summary>
    /// Options for a parse engine
    /// </summary>
    public interface IParseEngineOptions
    {
        /// <summary>
        /// Optimize right recursion during parsing
        /// </summary>
        bool OptimizeRightRecursion { get; }
    }

    /// <summary>
    /// Provides a communication layer between the ParseEngine and the parsing system, can generally be treated as the input
    /// for each Pulse to the parse engine and a place to push any generated data.
    /// </summary>
    public interface IParseContext : ILexContext
    {
        /// <summary>
        /// Callback method telling that the parse engine has been started for the specified state
        /// </summary>
        void Started(int origin, IState startState);

        /// <summary>
        /// Callback method telling that a state has been predicted for the specified token and state
        /// </summary>
        void Predicted(PredictionMode mode, int origin, IState predictState, IState nextState);

        /// <summary>
        /// Callback method telling that a state has been completed for the specified token and state
        /// </summary>
        void Completed(CompletionMode mode, int origin, IState completedState, IState nextState);

        /// <summary>
        /// Callback method telling that a token has scanned for the specified token
        /// </summary>
        void Scanned(int origin, IState scanState, IState nextState, IToken scannedToken);

        /// <summary>
        /// Callback method telling that a special "transition" has been made
        /// </summary>
        void Transitioned(int origin, ITransitionState transitionState);
    }
}