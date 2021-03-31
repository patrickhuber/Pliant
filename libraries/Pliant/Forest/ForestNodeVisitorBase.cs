namespace Pliant.Forest
{
    public abstract class ForestNodeVisitorBase : IForestNodeVisitor
    {

        public abstract void Visit(IIntermediateForestNode intermediateNode);

        public virtual void Visit(IAndForestNode andNode)
        {
            for (var c = 0; c < andNode.Children.Count; c++)
            {
                var child = andNode.Children[c];
                child.Accept(this);
            }
        }

        public abstract void Visit(ISymbolForestNode symbolNode);

        public virtual void Visit(ITerminalForestNode terminalNode) { }

        public virtual void Visit(ITokenForestNode tokenNode) { }
    }
}
