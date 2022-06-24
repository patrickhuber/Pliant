using System.Collections.Generic;
using Pliant.Grammars;
using Pliant.Charts;
using Pliant.Utilities;

namespace Pliant.Forest
{
    public class VirtualForestNode : InternalForestNode, ISymbolForestNode
    {
        private List<VirtualForestNodePath> _paths;

        private readonly int _hashCode;

        public override IReadOnlyList<IPackedForestNode> Children
        {
            get
            {
                if (ShouldLoadChildren())
                    LazyLoadChildren();
                return _children;
            }
        }
                
        public override ForestNodeType NodeType
        {
            get { return ForestNodeType.Symbol; }
        }

        public ISymbol Symbol { get; private set; }
        
        public VirtualForestNode(
            int location,
            ITransitionState transitionState,
            IForestNode completedParseNode)
            : this(
                  location,
                  transitionState, 
                  completedParseNode,
                  transitionState.GetTargetState())
        {
        }

        protected VirtualForestNode(
            int location,
            ITransitionState transitionState,
            IForestNode completedParseNode,
            IState targetState)
            : base(targetState.Origin, location)
        {
            _paths = new List<VirtualForestNodePath>();
            
            Symbol = targetState.DottedRule.Production.LeftHandSide;
            _hashCode = ComputeHashCode();
            var path = new VirtualForestNodePath(transitionState, completedParseNode);
            AddUniquePath(path);
        }
                
        public override void Accept(IForestNodeVisitor visitor)
        {
            visitor.Visit(this);
        }
        
        public void AddUniquePath(VirtualForestNodePath path)
        {
            if (!IsUniquePath(path))
                return;
            if (IsUniqueChildSubTree(path))
                CloneUniqueChildSubTree(path.ForestNode as IInternalForestNode);
        
            _paths.Add(path);
        }

        private bool IsUniquePath(VirtualForestNodePath path)
        {
            for (int p = 0; p < _paths.Count; p++)
            {
                var otherPath = _paths[p];
                if(path.Equals(otherPath))
                    return false;
            }
            return true;
        }

        private static bool IsUniqueChildSubTree(VirtualForestNodePath path)
        {
            var transitionState = path.TransitionState;
            var completedParseNode = path.ForestNode;

            return transitionState.Top.ParseNode != null
            && completedParseNode == transitionState.Top.ParseNode
            && (completedParseNode.NodeType == ForestNodeType.Intermediate
                || completedParseNode.NodeType == ForestNodeType.Symbol);
        }

        private void CloneUniqueChildSubTree(IInternalForestNode internalCompletedParseNode)
        {
            for (var a = 0; a < internalCompletedParseNode.Children.Count; a++)
            {
                var packedNode = internalCompletedParseNode.Children[a];
                var newPackedNode = new PackedForestNode();
                for (var c = 0; c < packedNode.Children.Count; c++)
                {
                    var child = packedNode.Children[c];
                    newPackedNode.AddChild(child);
                }
                _children.Add(newPackedNode);
            }
        }

        private bool ShouldLoadChildren()
        {
            return _children.Count == 0;
        }

        private void LazyLoadChildren()
        {
            for (int i = 0; i < _paths.Count; i++)
                LazyLoadPath(_paths[i]);
        }

        private void LazyLoadPath(VirtualForestNodePath path)
        {
            var transitionState = path.TransitionState;
            var completedParseNode = path.ForestNode;
            if (transitionState.Next != null)
            {
                var virtualNode = new VirtualForestNode(Location, transitionState.Next, completedParseNode);

                if (transitionState.Top.ParseNode is null)
                    AddUniqueFamily(virtualNode);
                else
                    AddUniqueFamily(transitionState.Top.ParseNode, virtualNode);
            }
            else if (!(transitionState.Top.ParseNode is null))
            {
                AddUniqueFamily(transitionState.Top.ParseNode, completedParseNode);
            }
            else
            {
                AddUniqueFamily(completedParseNode);
            }
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
