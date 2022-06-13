namespace Pliant.Forest
{
    public abstract class ForestNodeVisitorBase : IForestNodeVisitor
    {

        public abstract void Visit(IIntermediateForestNode intermediateNode);

        public virtual void Visit(IPackedForestNode packedNode)
        {
            for (var c = 0; c < packedNode.Children.Count; c++)
            {
                var child = packedNode.Children[c];
                child.Accept(this);
            }
        }

        public abstract void Visit(ISymbolForestNode symbolNode);

        public virtual void Visit(ITerminalForestNode terminalNode) { }

        public virtual void Visit(ITokenForestNode tokenNode) { }
    }
}
