namespace Pliant.Forest
{
    public abstract class NodeVisitorBase : INodeVisitor
    {
        public INodeVisitorStateManager StateManager { get; private set; }

        protected NodeVisitorBase(INodeVisitorStateManager stateManager)
        {
            StateManager = stateManager;
        }
        
        public virtual void Visit(IIntermediateNode intermediateNode)
        {
            var currentAndNode = StateManager.GetCurrentAndNode(intermediateNode);
            Visit(currentAndNode);
            StateManager.MarkAsTraversed(intermediateNode);
        }

        public virtual void Visit(ITokenNode tokenNode)
        { }

        public virtual void Visit(IAndNode andNode)
        {
            foreach (var child in andNode.Children)
                child.Accept(this);
        }

        public virtual void Visit(ISymbolNode symbolNode)
        {
            var currentAndNode = StateManager.GetCurrentAndNode(symbolNode);
            Visit(currentAndNode);
            StateManager.MarkAsTraversed(symbolNode);
        }

        public virtual void Visit(ITerminalNode terminalNode)
        { }
    }
}