using Pliant.Forest;
using System;
using System.Collections.Generic;

namespace Pliant.Tests.Common.Forest
{
    public abstract class FakeInternalForestNode : IInternalForestNode
    {
        private List<IPackedForestNode> _children;

        protected FakeInternalForestNode(int origin, int location, params IPackedForestNode[] children)
        {
            Origin = origin;
            Location = location;
            _children = new List<IPackedForestNode>(children);
        }

        public IReadOnlyList<IPackedForestNode> Children
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
