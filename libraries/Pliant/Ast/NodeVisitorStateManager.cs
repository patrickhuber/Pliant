using System;
using System.Collections.Generic;

namespace Pliant.Ast
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

        private int GetTraversalPosition(IInternalNode internalNode)
        {
            int value = 0;
            if (_stateStore.TryGetValue(internalNode, out value))
                return value;
            SetTraversalPosition(internalNode, value);
            return value;
        }

        public IAndNode GetCurrentAndNode(IInternalNode internalNode)
        {
            int value = GetTraversalPosition(internalNode);
            return internalNode.Children[value];
        }

        public void MarkAsTraversed(IInternalNode internalNode)
        {
            int value = GetTraversalPosition(internalNode);
            if (HasMoreTransitions(internalNode) && TryAcquireLock(internalNode))
                SetTraversalPosition(internalNode, value + 1);
            else
                TryReleaseLock(internalNode);
        }

        private void SetTraversalPosition(IInternalNode internalNode, int value)
        {
            _stateStore[internalNode] = value ;
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

        private bool HasMoreTransitions(IInternalNode internalNode)
        {
            return HasMoreTransitions(
                internalNode,
                GetTraversalPosition(internalNode));
        }

        private bool HasMoreTransitions(IInternalNode internalNode, int currentPosition)
        {
            return currentPosition < internalNode.Children.Count - 1;
        }
    }
}
