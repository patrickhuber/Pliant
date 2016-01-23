namespace Pliant.Ast
{
    public abstract class NodeVisitorBase : INodeVisitor
    {
        public INodeVisitorStateManager StateManager { get; private set; }

        protected NodeVisitorBase(INodeVisitorStateManager stateManager)
        {
            StateManager = stateManager;
        }

        protected NodeVisitorBase()
            : this(new NodeVisitorStateManager())
        {
        }

        public virtual void Visit(IIntermediateNode node)
        {
            var currentAndNode = StateManager.GetCurrentAndNode(node);
            Visit(currentAndNode);
            StateManager.MarkAsTraversed(node);
        }

        public virtual void Visit(ITokenNode tokenNode)
        { }

        public virtual void Visit(IAndNode andNode)
        {
            foreach (var child in andNode.Children)
                child.Accept(this);
        }

        public virtual void Visit(ISymbolNode node)
        {
            var currentAndNode = StateManager.GetCurrentAndNode(node);
            Visit(currentAndNode);
            StateManager.MarkAsTraversed(node);
        }

        public virtual void Visit(ITerminalNode node)
        { }
    }
}