using Pliant.Forest;
using System.Collections.Generic;

namespace Pliant.Tests.Common.Forest
{
    public class FakeAndForestNode : IPackedForestNode
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

        public void Accept(IForestNodeVisitor visitor)
        {
            for (int i = 0; i < Children.Count; i++)
            {
                var child = Children[i];
                child.Accept(visitor);
            }
        }

        public void Add(IForestNode child)
        {
            _children.Add(child);
        }


    }
}
