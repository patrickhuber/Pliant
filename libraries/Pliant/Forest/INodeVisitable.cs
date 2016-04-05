namespace Pliant.Forest
{
    public interface INodeVisitable
    {
        void Accept(INodeVisitor visitor);
    }
}