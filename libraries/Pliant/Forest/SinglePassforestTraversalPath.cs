namespace Pliant.Forest
{
    public class SinglePassForestTraversalPath : IForestTraversalPath
    {
        public IAndForestNode GetCurrentAndNode(IInternalForestNode internalNode)
        {
            return internalNode.Children[0];
        }
    }
}
