namespace Pliant.Tree
{
    public interface ITreeNode
    {
        TreeNodeType NodeType { get; }
        int Origin { get; }
        int Location { get; }

        void Accept(ITreeNodeVisitor visitor);
    }
}