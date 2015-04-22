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
        /// A single AND node. Virtual nodes are leo nodes and by nature don't have ambiguity.
        /// </summary>
        private AndNode _andNode;

        public int Location { get; private set; }
        
        public VirtualNode(int location, ITransitionState transitionState)
        {
            _transitionState = transitionState;
            _children = new ReadWriteList<IAndNode>();
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
        
        public IReadOnlyList<IAndNode> Children
        {
            get 
            {
                if(!ResultCached())
                    LazyLoadChildren();
                return _children; 
            }
        }

        private void LazyLoadChildren()
        {
            if (_transitionState.NextTransition != null)
            {
                var virtualNode = new VirtualNode(Location, _transitionState.NextTransition);
                if (_transitionState.Reduction.ParseNode == null)
                    AddUniqueFamily(virtualNode);
                else
                    AddUniqueFamily(_transitionState.Reduction.ParseNode, virtualNode);
            }
            else if (_transitionState.Reduction.ParseNode != null)
            {
                AddUniqueFamily(_transitionState.Reduction.ParseNode);
            }
            else { throw new Exception("Unreachable code detected. Unable to lazy load children of invalid virtual node."); }
        }

        private bool ResultCached()
        {
            return _andNode != null;
        }

        public void AddUniqueFamily(INode trigger)
        {
            if (_andNode != null)
                return;
            _andNode = new AndNode();
            _andNode.AddChild(trigger);
            _children.Add(_andNode);
        }

        public void AddUniqueFamily(INode source, INode trigger)
        {
            if (_andNode != null)
                return;
            _andNode = new AndNode();
            _andNode.AddChild(source);
            _andNode.AddChild(trigger);
            _children.Add(_andNode);
        }
    }
}
