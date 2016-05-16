namespace Pliant.Forest
{
    /// <summary>
    /// Implements a single pass node visitor state manager. Basically only
    /// returns the first IAndNode in the IInternalNode.Children collection.
    /// </summary>
    public class SinglePassForestNodeVisitorStateManager : IForestNodeVisitorStateManager
    {
        public IAndForestNode GetCurrentAndNode(IInternalForestNode internalNode)
        {
            return internalNode.Children[0];
        }

        public void MarkAsTraversed(IInternalForestNode internalNode)
        {
        }
    }
}