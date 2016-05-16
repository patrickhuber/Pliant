namespace Pliant.Forest
{
    public interface IForestNodeVisitable
    {
        void Accept(IForestNodeVisitor visitor);
    }
}