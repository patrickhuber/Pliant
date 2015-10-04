using Pliant.Grammars;

namespace Pliant.Nodes
{
    public interface ISymbolNode : IInternalNode
    {
        ISymbol Symbol { get;}
    }
}
