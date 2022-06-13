namespace Pliant.Forest
{
    public interface IForestNodeVisitor
    {
        void Visit(ITerminalForestNode node);

        void Visit(ISymbolForestNode node);

        void Visit(IIntermediateForestNode node);

        void Visit(IPackedForestNode packedNode);

        void Visit(ITokenForestNode tokenNode);
    }
}