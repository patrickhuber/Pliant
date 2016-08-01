using System;
using Pliant.Grammars;
using Pliant.Utilities;

namespace Pliant.Forest
{
    public class SymbolForestNode : InternalForestNode, ISymbolForestNode
    {
        public ISymbol Symbol { get; private set; }

        public SymbolForestNode(ISymbol symbol, int origin, int location)
            : base(origin, location)
        {
            Symbol = symbol;
            _hashCode = ComputeHashCode();
        }

        public override ForestNodeType NodeType
        {
            get { return ForestNodeType.Symbol; }
        }

        public override string ToString()
        {
            return $"({Symbol}, {Origin}, {Location})";
        }

        public override void Accept(IForestNodeVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            var symbolNode = obj as ISymbolForestNode;
            if (symbolNode == null)
                return false;

            return Location == symbolNode.Location
                && NodeType == symbolNode.NodeType
                && Origin == symbolNode.Origin
                && Symbol.Equals(symbolNode.Symbol);
        }

        private readonly int _hashCode;

        private int ComputeHashCode()
        {
            return HashCode.Compute(
                ((int)NodeType).GetHashCode(),
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