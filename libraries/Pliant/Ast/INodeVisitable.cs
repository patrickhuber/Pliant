namespace Pliant.Ast
{
    public interface INodeVisitable
    {
        void Accept(INodeVisitor visitor);
    }
}