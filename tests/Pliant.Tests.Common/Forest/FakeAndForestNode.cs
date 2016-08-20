using Pliant.Collections;
using Pliant.Forest;
using System.Collections.Generic;

namespace Pliant.Tests.Common.Forest
{
    public class FakeAndForestNode : IAndForestNode
    {
        private List<IForestNode> _children;

        public FakeAndForestNode(params IForestNode[] children)
        {
            _children = new List<IForestNode>(children);
        }

        public IReadOnlyList<IForestNode> Children
        {
            get
            {
                return _children;
            }
        }

        public void Add(IForestNode child)
        {
            _children.Add(child);
        }
    }
}
