namespace Pliant.Tree
{
    public abstract class TreeNodeVisitorBase : ITreeNodeVisitor
    {
        public virtual void Visit(IInternalTreeNode node)
        {
            var nodeChildrenCount = node.Children.Count;
            for (var i = 0; i < nodeChildrenCount; i++)
                node.Children[i].Accept(this);            
        }

        public virtual void Visit(ITokenTreeNode node)
        {
        }
    }
}