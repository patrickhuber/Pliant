using System;
using Pliant.Grammars;

namespace Pliant.Forest
{
    public class SymbolNode : InternalNode, ISymbolNode
    {
        public ISymbol Symbol { get; private set; }

        public SymbolNode(ISymbol symbol, int origin, int location)
            : base(origin, location)
        {
            Symbol = symbol;
            _hashCode = ComputeHashCode();
        }

        public override NodeType NodeType
        {
            get { return NodeType.Symbol; }
        }

        public override string ToString()
        {
            return $"({Symbol}, {Origin}, {Location})";
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }
        public override bool Equals(object obj)
        {
            if ((object)obj == null)
                return false;

            var symbolNode = obj as SymbolNode;
            if ((object)symbolNode == null)
                return false;

            return Location == symbolNode.Location
                && NodeType == symbolNode.NodeType
                && Origin == symbolNode.Origin
                && Symbol.Equals(symbolNode.Symbol);
        }

        private readonly int _hashCode;

        private int ComputeHashCode()
        {
            return HashUtil.ComputeHash(
                NodeType.GetHashCode(),
                Location.GetHashCode(),
                Origin.GetHashCode(),
                Symbol.GetHashCode());
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }
    }
}