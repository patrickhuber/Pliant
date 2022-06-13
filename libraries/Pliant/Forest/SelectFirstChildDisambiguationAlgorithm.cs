namespace Pliant.Forest
{
    /// <summary>
    /// Implements a disambiguation algorithm that always selects the firest child of an ambiguity.
    /// </summary>
    public class SelectFirstChildDisambiguationAlgorithm : IForestDisambiguationAlgorithm
    {
        public IPackedForestNode GetCurrentPackedNode(IInternalForestNode internalNode)
        {
            return internalNode.Children[0];
        }
    }
}