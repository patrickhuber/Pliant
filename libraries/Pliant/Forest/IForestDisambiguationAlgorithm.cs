namespace Pliant.Forest
{
    public interface IForestDisambiguationAlgorithm
    {
        IPackedForestNode GetCurrentPackedNode(IInternalForestNode internalNode);
    }
}