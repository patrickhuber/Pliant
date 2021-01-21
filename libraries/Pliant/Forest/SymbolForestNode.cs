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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0601:Value type to reference type conversion causing boxing allocation", Justification = "ToString is not called in performance critical code")]
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
            if (obj is null)
                return false;

            if (!(obj is ISymbolForestNode symbolNode))
                return false;

            return Location == symbolNode.Location
                && Origin == symbolNode.Origin
                && NodeType == symbolNode.NodeType                
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