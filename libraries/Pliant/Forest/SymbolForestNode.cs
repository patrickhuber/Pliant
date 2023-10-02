using Pliant.Grammars;
using Pliant.Utilities;
using System.Collections.Generic;
using System.IO;

namespace Pliant.Forest
{
    public class SymbolForestNode : InternalForestNode, ISymbolForestNode
    {
        public ISymbol Symbol { get; private set; }

        private bool _expanded = false;
        private IDictionary<IDynamicForestNodePath, IForestNode> _paths;

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
            // load the children 
            _ = Children;
            base.Accept(visitor);
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;

            if (obj is not ISymbolForestNode symbolNode)
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

        public void AddPath(IDynamicForestNodePath path, IForestNode node)
        {
            _paths ??= new Dictionary<IDynamicForestNodePath, IForestNode>();
            _paths[path] = node;
        }

        private void Expand()
        {
            if (_paths == null)
                return;

            foreach (var kvp in _paths)
            {
                var path = kvp.Key;
                var node = kvp.Value;

                var leftTree = path.Node();
                var rightSubTree = node;
                var next = path.Next();

                // this is the end of the chain
                if (next == null
                    || next.Node() == null
                    || next.Node().Location == rightSubTree.Location)
                {
                    AddUniqueFamily(leftTree, rightSubTree);
                    return;
                }

                var rightTree = new SymbolForestNode(Symbol, next.Node().Origin, Location);
                rightTree.AddPath(next, node);
                AddUniqueFamily(leftTree, rightTree);
            }
        }

        public override IReadOnlyList<IPackedForestNode> Children
        {
            get 
            {
                if (!_expanded) 
                {
                    Expand();
                    _expanded = true;
                }
                return _children;
            }
        }
    }
}