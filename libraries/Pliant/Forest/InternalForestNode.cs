using System.Collections.Generic;

namespace Pliant.Forest
{
    public abstract class InternalForestNode : ForestNodeBase, IInternalForestNode
    {
        protected readonly List<IPackedForestNode> _children;

        public virtual IReadOnlyList<IPackedForestNode> Children { get { return _children; } }

        protected InternalForestNode(int origin, int location)
            : base(origin, location)
        {
            _children = new List<IPackedForestNode>();
        }

        public void AddUniqueFamily(IForestNode trigger)
        {
            AddUniquePackedNode(trigger);
        }

        public void AddUniqueFamily(IForestNode source, IForestNode trigger)
        {
            if(source == this)
                source = Children[0].Children[0];
            AddUniquePackedNode(source, trigger);
        }        

        private void AddUniquePackedNode(IForestNode child)
        {
            AddUniquePackedNode(child, null);
        }

        private void AddUniquePackedNode(IForestNode firstChild, IForestNode secondChild)
        {
            var childCount = 1 + ((secondChild is null) ? 0 : 1);

            for (var c = 0; c < _children.Count; c++)
            {
                var packedNode = _children[c];

                if (packedNode.Children.Count != childCount)
                    continue;

                if (IsMatchedSubTree(firstChild, secondChild, packedNode))
                    return;
            }

            // not found so return new and node
            var newPackedNode = new PackedForestNode();
            newPackedNode.AddChild(firstChild);
            if (childCount > 1)
                newPackedNode.AddChild(secondChild);

            _children.Add(newPackedNode);
        }

        private static bool IsMatchedSubTree(IForestNode firstChild, IForestNode secondChild, IPackedForestNode packedNode)
        {
            var firstCompareNode = packedNode.Children[0];

            // if first child matches the compare node, continue
            // otherwise return false
            if (!firstChild.Equals(firstCompareNode))
                return false;

            if (secondChild is null)
                return true;

            var secondCompareNode = packedNode.Children[1];

            // return true if the second child matches
            // otherwise return false
            return secondChild.Equals(secondCompareNode);
        }

        public override void Accept(IForestNodeVisitor visitor)
        {
            var childrenCount = _children.Count;
            for (var i = 0; i < childrenCount; i++)
            {
                var child = _children[i];
                child.Accept(visitor);
            }
        }
    }
}