namespace Pliant.Forest
{
    public interface IForestNodeVisitorStateManager
    {
        IAndForestNode GetCurrentAndNode(IInternalForestNode internalNode);

        void MarkAsTraversed(IInternalForestNode internalNode);
    }
}