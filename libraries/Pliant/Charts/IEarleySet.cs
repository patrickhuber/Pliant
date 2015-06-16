using Pliant.Grammars;
using System.Collections.Generic;

namespace Pliant.Charts
{
    public interface IEarleySet
    {
        IReadOnlyList<IState> Predictions { get; }
        IReadOnlyList<IState> Scans { get; }
        IReadOnlyList<IState> Completions { get; }
        IReadOnlyList<IState> Transitions { get; }
        bool Enqueue(IState state);
        int Location { get; }
        ITransitionState FindTransitionState(ISymbol searchSymbol);
        IState FindSourceState(ISymbol searchSymbol);
    }
}
