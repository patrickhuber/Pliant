using Pliant.Collections;
using Pliant.Forest;
using System;
using System.Collections.Generic;

namespace Pliant.Tests.Unit.Forest
{
    public class FakeAndForestNode : IAndForestNode
    {
        private ReadWriteList<IForestNode> _children;

        public FakeAndForestNode(params IForestNode[] children)
        {
            _children = new ReadWriteList<IForestNode>(children);
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
