using Pliant.Collections;
using System.Collections.Generic;

namespace Pliant.Forest
{
    public abstract class InternalForestNode : ForestNodeBase, IInternalForestNode
    {
        protected readonly List<IAndForestNode> _children;
        public virtual IReadOnlyList<IAndForestNode> Children { get { return _children; } }

        protected InternalForestNode(int origin, int location)
            : base(origin, location)
        {
            _children = new List<IAndForestNode>();
        }

        public void AddUniqueFamily(IForestNode trigger)
        {
            AddUniqueAndNode(trigger);
        }

        public void AddUniqueFamily(IForestNode source, IForestNode trigger)
        {
            if(source == this)
                source = Children[0].Children[0];
            AddUniqueAndNode(source, trigger);
        }        

        private void AddUniqueAndNode(IForestNode child)
        {
            AddUniqueAndNode(child, null);
        }

        private void AddUniqueAndNode(IForestNode firstChild, IForestNode secondChild)
        {
            var childCount = 1 + ((secondChild == null) ? 0 : 1);

            for (var c = 0; c < _children.Count; c++)
            {
                var andNode = _children[c];

                if (andNode.Children.Count != childCount)
                    continue;

                if (IsMatchedSubTree(firstChild, secondChild, andNode))
                    return;
            }

            // not found so return new and node
            var newAndNode = new AndForestNode();
            newAndNode.AddChild(firstChild);
            if (childCount > 1)
                newAndNode.AddChild(secondChild);

            _children.Add(newAndNode);
        }

        private static bool IsMatchedSubTree(IForestNode firstChild, IForestNode secondChild, IAndForestNode andNode)
        {
            var firstCompareNode = andNode.Children[0];

            // if first child matches the compare node, continue
            // otherwise return false
            if (!firstChild.Equals(firstCompareNode))
                return false;

            if (secondChild == null)
                return true;

            var secondCompareNode = andNode.Children[1];

            // return true if the second child matches
            // otherwise return false
            return secondChild.Equals(secondCompareNode);
        }              
    }
}