using Pliant.Grammars;

namespace Pliant.Ast
{
    public interface ISymbolNode : IInternalNode
    {
        ISymbol Symbol { get;}
    }
}
