using Pliant.Grammars;

namespace Pliant.Forest
{
    public interface ISymbolNode : IInternalNode
    {
        ISymbol Symbol { get; }
    }
}