namespace Pliant.Nodes
{
    public abstract class NodeBase : INode
    {
        public virtual int Location { get; protected set; }

        public virtual NodeType NodeType { get; protected set; }

        public virtual int Origin { get; protected set; }

        public abstract void Accept(INodeVisitor visitor, INodeVisitorStateManager stateManager);
    }
}
