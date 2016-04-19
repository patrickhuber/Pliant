using Pliant.Forest;
using Pliant.Grammars;

namespace Pliant.Tests.Unit.Forest
{
    public class FakeSymbolNode : FakeInternalNode, ISymbolNode
    {
        public FakeSymbolNode(ISymbol symbol, int origin, int location, params IAndNode[] children) 
            : base(origin, location, children)
        {
            Symbol = symbol;
        }

        public FakeSymbolNode(string symbol, int origin, int location, params IAndNode[] children)
            : this(new NonTerminal(symbol), origin, location, children)
        {
        }

        public override NodeType NodeType
        {
            get { return NodeType.Symbol; }
        }

        public ISymbol Symbol { get; private set; }
    }
}
