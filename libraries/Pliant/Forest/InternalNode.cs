using Pliant.Collections;
using System;
using System.Collections.Generic;

namespace Pliant.Forest
{
    public abstract class InternalNode : NodeBase, IInternalNode
    {
        private ReadWriteList<IAndNode> _children;

        public IReadOnlyList<IAndNode> Children { get { return _children; } }

        protected InternalNode(int origin, int location)
            : base(origin, location)
        {
            _children = new ReadWriteList<IAndNode>();
        }

        public void AddUniqueFamily(INode trigger)
        {
            AddUniqueAndNode(trigger);
        }

        public void AddUniqueFamily(INode source, INode trigger)
        {
            if(source == this)
                source = Children[0].Children[0];
            AddUniqueAndNode(source, trigger);
        }

        private void AddUniqueAndNode(params INode[] children)
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
            var newAndNode = new AndNode();
            foreach (var child in children)
                newAndNode.AddChild(child);

            _children.Add(newAndNode);
        }

        private static bool IsMatchedSubTree(INode[] children, IAndNode andNode)
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