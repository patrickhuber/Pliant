namespace Pliant.Forest
{
    public interface INodeVisitorStateManager
    {
        IAndNode GetCurrentAndNode(IInternalNode internalNode);

        void MarkAsTraversed(IInternalNode internalNode);
    }
}