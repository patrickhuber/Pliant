using Pliant.Grammars;

namespace Pliant.Forest
{
    public interface ISymbolForestNode : IInternalForestNode
    {
        ISymbol Symbol { get; }
        void AddPath(IDynamicForestNodePath path, IForestNode node);
    }
}