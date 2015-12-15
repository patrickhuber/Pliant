namespace Pliant.Ast
{
    public interface INode : INodeVisitable
    {
        int Origin { get; }

        int Location { get; }

        NodeType NodeType { get; }
    }
}