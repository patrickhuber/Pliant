using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant.Nodes
{
    public class NodeVisitorStateManager : INodeVisitorStateManager
    {
        private IInternalNode _lock;
        private IDictionary<IInternalNode, int> _stateStore;
        
        public NodeVisitorStateManager()
        {
            _lock = null;
            _stateStore = new Dictionary<IInternalNode, int>();
        }

        public IAndNode GetCurrentAndNode(IInternalNode internalNode)
        {
            int value = 0;
            if (_stateStore.TryGetValue(internalNode, out value))
                return internalNode.Children[value];
            return internalNode.Children[0];
        }

        public void MarkAsTraversed(IInternalNode internalNode)
        {
            int value = 0;
            if (!_stateStore.TryGetValue(internalNode, out value))
                _stateStore[internalNode] = 0;
            if (value < internalNode.Children.Count - 1 && TryAcquireLock(internalNode))
                _stateStore[internalNode] = value + 1;
            else
                TryReleaseLock(internalNode);
        }

        private bool TryAcquireLock(IInternalNode internalNode)
        {
            if (_lock != null)
                return false;
            _lock = internalNode;
            return true;
        }

        private bool TryReleaseLock(IInternalNode internalNode)
        {
            if (_lock != null && !_lock.Equals(internalNode))
                return false;
            _lock = null;
            return true;
        }
    }
}
