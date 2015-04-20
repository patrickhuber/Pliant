using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant
{
    public class VirtualNode : IInternalNode
    {
        private ITransitionState _transitionState;

        private ReadWriteList<IAndNode> _children;
        /// <summary>
        /// A single and node. Virtual nodes are leo nodes and by nature don't have ambiguity.
        /// </summary>
        private AndNode _andNode;

        public int Location { get; private set; }


        public VirtualNode(int location, ITransitionState transitionState)
        {
            _transitionState = transitionState;
            _children = new ReadWriteList<IAndNode>();
            _andNode = new AndNode();
            _children.Add(_andNode);
            Location = location;
        }

        public int Origin
        {
            get { return _transitionState.Origin; }
        }
             
        public NodeType NodeType
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsEmpty
        {
            get { throw new NotImplementedException(); }
        }

        public IReadOnlyList<IAndNode> Children
        {
            get 
            {
                LazyLoadChildren();
                return _children; 
            }
        }

        private void LazyLoadChildren()
        {
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
