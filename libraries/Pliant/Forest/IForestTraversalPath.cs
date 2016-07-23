namespace Pliant.Forest
{
    public interface IForestTraversalPath
    {
        IAndForestNode GetCurrentAndNode(IInternalForestNode internalNode);
    }
}
