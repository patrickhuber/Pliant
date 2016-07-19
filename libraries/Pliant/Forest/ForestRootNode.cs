using Pliant.Collections;
using System.Collections.Generic;

namespace Pliant.Forest
{
    public class ForestRootNode : IForestRootNode
    {
        private ReadWriteList<IAndForestNode> _children;

        public IReadOnlyList<IAndForestNode> Children { get { return _children; } }

        public ForestRootNode()
        {
            _children = new ReadWriteList<IAndForestNode>();
        }

        public void AddChild(IAndForestNode andForestNode)
        {
            _children.Add(andForestNode);
        }
    }
}
