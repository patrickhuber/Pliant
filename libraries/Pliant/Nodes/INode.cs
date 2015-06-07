namespace Pliant.Nodes
{
    public interface INode
    {
        int Origin { get; }

        int Location { get; }

        NodeType NodeType { get; }
    }
}
