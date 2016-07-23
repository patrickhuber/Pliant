namespace Pliant.Forest
{
    public interface IForestTraversalPathProvider
    {
        bool MoveNext();
        IForestTraversalPath Current { get; }
    }
}
