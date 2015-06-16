using Pliant.Collections;
using System.Collections.Generic;

namespace Pliant.Nodes
{
    public class AndNode : IAndNode
    {
        public IReadOnlyList<INode> Children { get { return _children; } }

        private ReadWriteList<INode> _children;

        public AndNode()
        {
            _children = new ReadWriteList<INode>();
        }

        public void AddChild(INode orNode)
        {
            _children.Add(orNode);
        }
    }
}
