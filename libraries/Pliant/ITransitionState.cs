using System;
namespace Pliant
{
    public interface ITransitionState : IState
    {
        ISymbol Recognized { get; }
        
        IState Reduction { get; }

        ITransitionState PreviousTransition { get;  }
    }
}
