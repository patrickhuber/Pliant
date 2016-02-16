namespace Pliant.Tree
{
    public abstract class TreeNodeVisitorBase : ITreeNodeVisitor
    {
        public virtual void Visit(IInternalTreeNode node)
        {
            foreach (var child in node.Children)
                child.Accept(this);
        }

        public virtual void Visit(ITokenTreeNode node)
        {
        }
    }
}