using Pliant.Collections;
using Pliant.Forest;
using System;
using System.Collections.Generic;

namespace Pliant.Tests.Unit.Forest
{
    public abstract class FakeInternalNode : IInternalNode
    {
        private ReadWriteList<IAndNode> _children;

        protected FakeInternalNode(int origin, int location, params IAndNode[] children)
        {
            Origin = origin;
            Location = location;
            _children = new ReadWriteList<IAndNode>(children);
        }

        public IReadOnlyList<IAndNode> Children
        {
            get { return _children; }
        }

        public int Location { get; private set; }

        public abstract NodeType NodeType { get; }

        public int Origin { get; private set; }

        public void Accept(INodeVisitor visitor)
        {
            throw new NotImplementedException();
        }

        public void AddUniqueFamily(INode trigger)
        {
            throw new NotImplementedException();
        }

        public void AddUniqueFamily(INode source, INode trigger)
        {
            throw new NotImplementedException();
        }
    }
}
