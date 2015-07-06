using Pliant.Grammars;

namespace Pliant.Charts
{
    public interface ITransitionState : IState
    {
        ISymbol Recognized { get; }
        
        IState Reduction { get; }

        ITransitionState NextTransition { get; set; }
    }
}
