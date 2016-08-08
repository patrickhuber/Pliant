namespace Pliant.Forest
{
    public interface IForestDisambiguationAlgorithm
    {
        IAndForestNode GetCurrentAndNode(IInternalForestNode internalNode);
    }
}