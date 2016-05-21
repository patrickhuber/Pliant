using Pliant.Collections;
using System.Collections.Generic;

namespace Pliant.Forest
{
    public class AndForestNode : IAndForestNode
    {
        public IReadOnlyList<IForestNode> Children { get { return _children; } }

        private readonly ReadWriteList<IForestNode> _children;

        public AndForestNode()
        {
            _children = new ReadWriteList<IForestNode>();
        }

        public void AddChild(IForestNode orNode)
        {
            _children.Add(orNode);
        }
    }
}