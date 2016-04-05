namespace Pliant.Forest
{
    /// <summary>
    /// Implements a single pass node visitor state manager. Basically only
    /// returns the first IAndNode in the IInternalNode.Children collection.
    /// </summary>
    public class SinglePassNodeVisitorStateManager : INodeVisitorStateManager
    {
        public IAndNode GetCurrentAndNode(IInternalNode internalNode)
        {
            return internalNode.Children[0];
        }

        public void MarkAsTraversed(IInternalNode internalNode)
        {
        }
    }
}