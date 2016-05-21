using Pliant.Grammars;

namespace Pliant.Forest
{
    public interface ISymbolForestNode : IInternalForestNode
    {
        ISymbol Symbol { get; }
    }
}