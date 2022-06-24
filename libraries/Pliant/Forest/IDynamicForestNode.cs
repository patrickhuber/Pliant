namespace Pliant.Forest
{
    public interface IDynamicForestNode : ISymbolForestNode
    {
        IDynamicForestNodeLink Link { get; }
    }
}
