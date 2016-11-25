using System.Diagnostics;
using Pliant.Charts;
using Pliant.Tokens;

namespace Pliant.Runtime
{
    /// <summary>
    /// Default parse context implementation
    /// </summary>
    public class ParseContext : IParseContext, ILexContext
    {
        public ParseContext()
        {
        }

        public void ReadCharacter(int position, char character)
        {
        }

        public virtual void Started(int origin, IState startState)
        {
            Log("Start", origin, startState);
        }

        public virtual void Predicted(PredictionMode mode, int origin, IState predictState, IState nextState)
        {
            Log("Predict", origin, nextState);
        }

        public virtual void Completed(CompletionMode mode, int origin, IState completedState, IState nextState)
        {
            Log("Complete", origin, nextState);
        }

        public virtual void Scanned(int origin, IState scanState, IState nextState, IToken scannedToken)
        {
            LogScan(origin, nextState, scannedToken);
        }

        public virtual void Transitioned(int origin, ITransitionState transitionState)
        {
            Log("Transition", origin, transitionState);
        }

        #region Logging
        protected static void Log(string operation, int origin, IState state)
        {
            LogOriginStateOperation(operation, origin, state);
            Debug.WriteLine(string.Empty);
        }

        protected static void LogOriginStateOperation(string operation, int origin, IState state)
        {
            Debug.Write($"{origin.ToString().PadRight(50)}{state.ToString().PadRight(50)}{operation}");
        }

        protected static void LogScan(int origin, IState state, IToken token)
        {
            LogOriginStateOperation("Scan", origin, state);
            Debug.WriteLine($" {token.Value}");
        }
        #endregion
    }
}