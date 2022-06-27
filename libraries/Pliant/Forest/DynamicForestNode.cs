using Pliant.Grammars;
using Pliant.Utilities;
using System;
using System.Collections.Generic;

namespace Pliant.Forest
{

    public class DynamicForestNode : InternalForestNode, IDynamicForestNode
    {
        public ISymbol Symbol { get; private set; }

        /// <summary>
        /// Previous is the previous root node. Can be null.
        /// </summary>
        public IDynamicForestNodeLink Link { get; private set; }
        
        public override ForestNodeType NodeType => ForestNodeType.Symbol;

        /// <summary>
        /// the flag determining if the children are loaded
        /// </summary>
        private bool _childrenLoaded = false;
        

        private readonly int _hashCode;
        /// <summary>
        /// Constructs a dynamic parse node. It is possible for top and bottom to be the same node. The previous node can be null
        /// </summary>
        /// <example>
        /// Links
        /// --- 
        /// Top     = (E, 0, 4)
        /// Bottom  = (E, 3, 4)
        /// Previous
        ///     Top     = (E, 0, 3)
        ///     Bottom  = (E, 2, 3)
        ///     Previous 
        ///        Top      = (E, 0, 2)
        ///        Bottom   = (E, 1, 2)
        ///        Previous 
        ///            Top      = (E, 0, 1)
        ///            Bottom   = (E, 0, 1)
        ///            
        /// Breadth First Forest
        /// ----
        /// (E, 0, 5) -> (E, 0, 1) (E, 1, 5)
        /// (E, 0, 4) -> (E, 0, 1) (E, 1, 4)
        /// (E, 1, 5) -> (E, 1, 2) (E, 2, 5)
        /// (E, 0, 3) -> (E, 0, 1) (E, 1, 3)
        /// (E, 1, 4) -> (E, 1, 2) (E, 2, 4)
        /// (E, 0, 2) -> (E, 0, 1) (E, 1, 2)
        /// (E, 1, 3) -> (E, 1, 2) (E, 2, 3)        
        /// (E, 0, 1) -> (F, 0, 1)
        ///            | (F, 0, 1) (E, 1, 1)
        /// (E, 1, 2) -> (F, 1, 2)
        ///            | (F, 1, 2) (E, 2, 2)                
        /// (E, 2, 3) -> (F, 2, 3)
        ///            | (F, 2, 3) (E, 3, 3)
        /// (E, 3, 4) -> (F, 3, 4)
        ///            | (F, 3, 4) (E, 4, 4)
        /// (E, 4, 5) -> (F, 4, 5)
        ///            | (F, 4, 5) (E, 5, 5)
        /// Dynamic Nodes
        /// ----
        /// (E, 1, 3)
        /// (E, 1, 4)
        /// (E, 2, 4)
        /// (E, 1, 5)
        /// (E, 2, 5)
        /// (E, 3, 5)
        /// </example>
        /// <param name="location"></param>
        /// <param name="top">The top of the parse reduction chain.</param>
        /// <param name="bottom">The bottom of the parse reudction chain.</param>
        /// <param name="previous">the previous root node</param>
        public DynamicForestNode(IDynamicForestNodeLink link, int location)
            : base(link.Bottom.Origin, location)
        {
            Link = link;
            Symbol = link.Symbol;
            _hashCode = ComputeHashCode();
        }

        public override IReadOnlyList<IPackedForestNode> Children
        {
            get
            {
                if (_childrenLoaded)
                    return _children;
                LazyLoadChildren();
                _childrenLoaded = true;
                return _children;
            }
        }

        private void LazyLoadChildren()
        {
            var next = Link.Next;

            if (next is null)
                AddUniqueFamily(Link.Bottom);
            else
                AddUniqueFamily(Link.Bottom, new DynamicForestNode(next, Location));
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
                && NodeType == symbolNode.NodeType
                && Origin == symbolNode.Origin
                && Symbol.Equals(symbolNode.Symbol);
        }

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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0601:Value type to reference type conversion causing boxing allocation", Justification = "ToString is not called in performance critical code")]
        public override string ToString()
        {
            return $"({Symbol}, {Origin}, {Location})";
        }
    }
}
