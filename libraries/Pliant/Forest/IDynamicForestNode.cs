namespace Pliant.Forest
{
    public interface IDynamicForestNode : ISymbolForestNode
    {
        IDynamicForestNodeLink Current { get; }
    }
}
