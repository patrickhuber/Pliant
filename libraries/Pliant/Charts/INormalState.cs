using Pliant.Grammars;

namespace Pliant.Charts
{
    public interface INormalState : IState
    {
        bool IsSource(ISymbol searchSymbol);
    }
}
