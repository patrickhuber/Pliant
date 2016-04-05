namespace Pliant.Forest
{
    public interface INode : INodeVisitable
    {
        int Origin { get; }

        int Location { get; }

        NodeType NodeType { get; }
    }
}