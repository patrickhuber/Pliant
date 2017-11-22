using Pliant.Grammars;
using System.Collections.Generic;

namespace Pliant.Charts
{
    public interface INormalState : IState
    {
        bool IsSource(ISymbol searchSymbol);
    }
}
