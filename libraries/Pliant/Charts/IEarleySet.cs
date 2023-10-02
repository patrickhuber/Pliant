using Pliant.Grammars;
using System.Collections.Generic;

namespace Pliant.Charts
{
    public interface IEarleySet
    {
        IReadOnlyList<INormalState> Predictions { get; }

        IReadOnlyList<INormalState> Scans { get; }

        IReadOnlyList<INormalState> Completions { get; }

        IReadOnlyList<ITransitionState> Transitions { get; }

        bool Enqueue(IState state);

        int Location { get; }

        /// <summary>
        /// Returns the transition state with the same symbol as the search symbol
        /// </summary>
        /// <param name="searchSymbol"></param>
        /// <returns></returns>
        ITransitionState FindTransitionState(ISymbol searchSymbol);

        /// <summary>
        /// Returns all predictions where the postdot symbol is the same as the search symbol
        /// </summary>
        /// <param name="searchSymbol"></param>
        /// <returns></returns>
        IReadOnlyList<INormalState> FindSourceStates(ISymbol searchSymbol);

        /// <summary>
        /// Returns all completions where the left hand symbol is the same as the search symbol
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        IReadOnlyList<INormalState> FindReductions(ISymbol symbol);
    }
}