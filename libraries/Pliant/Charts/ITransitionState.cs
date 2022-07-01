using Pliant.Grammars;

namespace Pliant.Charts
{
    public interface ITransitionState : IState
    {
        ISymbol Recognized { get; }

        IState Bottom { get; }

        ITransitionState Next { get; set; }

        ITransitionState First { get; set; }
    }
}