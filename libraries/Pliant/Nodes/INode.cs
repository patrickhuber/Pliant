namespace Pliant.Nodes
{
    public interface INode : INodeVisitable
    {
        int Origin { get; }

        int Location { get; }

        NodeType NodeType { get; }
    }
}
