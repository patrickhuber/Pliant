using Pliant.Collections;
using Pliant.Forest;
using System;
using System.Collections.Generic;

namespace Pliant.Tests.Unit.Forest
{
    public class FakeAndNode : IAndNode
    {
        private ReadWriteList<INode> _children;

        public FakeAndNode(params INode[] children)
        {
            _children = new ReadWriteList<INode>(children);
        }

        public IReadOnlyList<INode> Children
        {
            get
            {
                return _children;
            }
        }

        public void Add(INode child)
        {
            _children.Add(child);
        }
    }
}
