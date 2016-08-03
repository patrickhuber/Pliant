namespace Pliant.Forest
{
    public abstract class ForestNodeVisitorBase : IForestNodeVisitor
    {
        public IForestDisambiguationAlgorithm ForestDisambiguationAlgorithm { get; private set; }

        protected ForestNodeVisitorBase(IForestDisambiguationAlgorithm forestDisambiguationAlgorithm)
        {
            ForestDisambiguationAlgorithm = forestDisambiguationAlgorithm;
        }

        public virtual void Visit(IIntermediateForestNode intermediateNode)
        {
            var currentAndNode = ForestDisambiguationAlgorithm.GetCurrentAndNode(intermediateNode);
            Visit(currentAndNode);
        }

        public virtual void Visit(ITokenForestNode tokenNode)
        { }

        public virtual void Visit(IAndForestNode andNode)
        {
            for (var c = 0; c < andNode.Children.Count; c++)
            {
                var child = andNode.Children[c];
                child.Accept(this);
            }
        }

        public virtual void Visit(ISymbolForestNode symbolNode)
        {
            var currentAndNode = ForestDisambiguationAlgorithm.GetCurrentAndNode(symbolNode);
            Visit(currentAndNode);
        }

        public virtual void Visit(ITerminalForestNode terminalNode)
        { }
    }
}