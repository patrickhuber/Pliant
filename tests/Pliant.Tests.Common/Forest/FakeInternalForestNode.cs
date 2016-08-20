using Pliant.Collections;
using Pliant.Forest;
using System;
using System.Collections.Generic;

namespace Pliant.Tests.Common.Forest
{
    public abstract class FakeInternalForestNode : IInternalForestNode
    {
        private List<IAndForestNode> _children;

        protected FakeInternalForestNode(int origin, int location, params IAndForestNode[] children)
        {
            Origin = origin;
            Location = location;
            _children = new List<IAndForestNode>(children);
        }

        public IReadOnlyList<IAndForestNode> Children
        {
            get { return _children; }
        }

        public int Location { get; private set; }

        public abstract ForestNodeType NodeType { get; }

        public int Origin { get; private set; }

        public void Accept(IForestNodeVisitor visitor)
        {
            throw new NotImplementedException();
        }

        public void AddUniqueFamily(IForestNode trigger)
        {
            throw new NotImplementedException();
        }

        public void AddUniqueFamily(IForestNode source, IForestNode trigger)
        {
            throw new NotImplementedException();
        }
    }
}
