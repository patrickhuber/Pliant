using Pliant.Forest;
using Pliant.Grammars;

namespace Pliant.Charts
{
    public interface ITransitionState : IState
    {
        ISymbol Recognized { get; }

        INormalState Reduction { get; }

        ITransitionState NextTransition { get; set; }

        int Index { get; }

        IState GetTargetState();
    }
}