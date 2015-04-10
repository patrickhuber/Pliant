using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant
{
    public abstract class InternalNode : IInternalNode
    {
        private ReadWriteList<INode> _children;

        public int Origin { get; private set; }
        public int Location { get; private set; }
        public IReadOnlyList<INode> Children { get { return _children; } }
        protected InternalNode(int origin, int location)
        {
            Origin = origin;
            Location = location;
            _children = new ReadWriteList<INode>();
        }

        public bool IsEmpty
        {
            get { return _children.Count == 0; }
        }

        public void AddChild(INode node)
        {
            if (_children.Count == 2)
                throw new Exception("maximum number of children is 2");
            _children.Add(node);
        }
    }
}
