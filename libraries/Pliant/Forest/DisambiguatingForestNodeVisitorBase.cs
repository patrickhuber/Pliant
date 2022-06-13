namespace Pliant.Forest
{
    public abstract class DisambiguatingForestNodeVisitorBase : ForestNodeVisitorBase
    {
        public IForestDisambiguationAlgorithm ForestDisambiguationAlgorithm { get; private set; }

        protected DisambiguatingForestNodeVisitorBase(IForestDisambiguationAlgorithm forestDisambiguationAlgorithm)
        {
            ForestDisambiguationAlgorithm = forestDisambiguationAlgorithm;
        }

        public override void Visit(IIntermediateForestNode intermediateNode)
        {
            var currentPackedNode = ForestDisambiguationAlgorithm.GetCurrentPackedNode(intermediateNode);
            Visit(currentPackedNode);
        }

        public override void Visit(ITokenForestNode tokenNode)
        { }

        public override void Visit(ISymbolForestNode symbolNode)
        {
            var currentPackedNode = ForestDisambiguationAlgorithm.GetCurrentPackedNode(symbolNode);
            Visit(currentPackedNode);
        }

        public override void Visit(ITerminalForestNode terminalNode)
        { }
    }
}