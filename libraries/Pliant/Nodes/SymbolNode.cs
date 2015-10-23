using Pliant.Grammars;

namespace Pliant.Nodes
{
    public class SymbolNode : InternalNode, ISymbolNode
    {
        public ISymbol Symbol { get; private set; }
        
        public SymbolNode(ISymbol symbol, int origin, int location)
            : base(origin, location)
        {            
            Symbol = symbol;
            NodeType = NodeType.Symbol;
        }

        public override string ToString()
        {
            return string.Format("({0}, {1}, {2})", Symbol, Origin, Location);
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
