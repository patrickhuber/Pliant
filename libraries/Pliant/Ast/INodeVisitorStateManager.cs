namespace Pliant.Ast
{
    public interface INodeVisitorStateManager
    {
        IAndNode GetCurrentAndNode(IInternalNode internalNode);

        void MarkAsTraversed(IInternalNode internalNode);
    }
}