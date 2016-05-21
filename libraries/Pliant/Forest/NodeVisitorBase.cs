namespace Pliant.Forest
{
    public abstract class NodeVisitorBase : IForestNodeVisitor
    {
        public IForestNodeVisitorStateManager StateManager { get; private set; }

        protected NodeVisitorBase(IForestNodeVisitorStateManager stateManager)
        {
            StateManager = stateManager;
        }
        
        public virtual void Visit(IIntermediateForestNode intermediateNode)
        {
            var currentAndNode = StateManager.GetCurrentAndNode(intermediateNode);
            Visit(currentAndNode);
            StateManager.MarkAsTraversed(intermediateNode);
        }

        public virtual void Visit(ITokenForestNode tokenNode)
        { }

        public virtual void Visit(IAndForestNode andNode)
        {
            foreach (var child in andNode.Children)
                child.Accept(this);
        }

        public virtual void Visit(ISymbolForestNode symbolNode)
        {
            var currentAndNode = StateManager.GetCurrentAndNode(symbolNode);
            Visit(currentAndNode);
            StateManager.MarkAsTraversed(symbolNode);
        }

        public virtual void Visit(ITerminalForestNode terminalNode)
        { }
    }
}