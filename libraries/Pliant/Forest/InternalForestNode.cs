using Pliant.Collections;
using System;
using System.Collections.Generic;

namespace Pliant.Forest
{
    public abstract class InternalForestNode : ForestNodeBase, IInternalForestNode
    {
        private ReadWriteList<IAndForestNode> _children;

        public IReadOnlyList<IAndForestNode> Children { get { return _children; } }

        protected InternalForestNode(int origin, int location)
            : base(origin, location)
        {
            _children = new ReadWriteList<IAndForestNode>();
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

        private void AddUniqueAndNode(params IForestNode[] children)
        {
            foreach (var andNode in _children)
            {
                if (andNode.Children.Count != children.Length)
                    continue;
                var isMatchedSubTree = IsMatchedSubTree(children, andNode);
                if (isMatchedSubTree)
                    return;
            }

            // not found so return new and node
            var newAndNode = new AndForestNode();
            foreach (var child in children)
                newAndNode.AddChild(child);

            _children.Add(newAndNode);
        }

        private static bool IsMatchedSubTree(IForestNode[] children, IAndForestNode andNode)
        {
            for (var c = 0; c < andNode.Children.Count; c++)
            {
                var parameterNode = children[c];
                var compareNode = andNode.Children[c];

                if (!parameterNode.Equals(compareNode))
                    return false;
            }
            return true;
        }        
    }
}