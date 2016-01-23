namespace Pliant.Ast
{
    public interface INodeVisitor
    {
        void Visit(ITerminalNode node);

        void Visit(ISymbolNode node);

        void Visit(IIntermediateNode node);

        void Visit(IAndNode andNode);

        void Visit(ITokenNode tokenNode);
    }
}