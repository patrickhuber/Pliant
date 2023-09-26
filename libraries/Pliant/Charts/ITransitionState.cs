using Pliant.Forest;
using Pliant.Grammars;

namespace Pliant.Charts
{
    public interface ITransitionState : IState, IDynamicForestNodePath
    {
        ISymbol Symbol { get; }

        ITransitionState NextTransition { get; set; }

        int Root { get; set; }
    }
}