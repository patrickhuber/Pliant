using System.Collections.Generic;

namespace Pliant.Forest
{
    public class MultiPassForestNodeVisitorStateManager : IForestNodeVisitorStateManager
    {
        private IInternalForestNode _lock;
        private Dictionary<IInternalForestNode, int> _stateStore;

        public MultiPassForestNodeVisitorStateManager()
        {
            _lock = null;
            _stateStore = new Dictionary<IInternalForestNode, int>();
        }

        private int GetTraversalPosition(IInternalForestNode internalNode)
        {
            var value = 0;
            if (_stateStore.TryGetValue(internalNode, out value))
                return value;
            SetTraversalPosition(internalNode, value);
            return value;
        }

        public IAndForestNode GetCurrentAndNode(IInternalForestNode internalNode)
        {
            var value = GetTraversalPosition(internalNode);
            return internalNode.Children[value];
        }

        public void MarkAsTraversed(IInternalForestNode internalNode)
        {
            var value = GetTraversalPosition(internalNode);
            if (HasMoreTransitions(internalNode) && TryAcquireLock(internalNode))
                SetTraversalPosition(internalNode, value + 1);
            else
                TryReleaseLock(internalNode);
        }

        private void SetTraversalPosition(IInternalForestNode internalNode, int value)
        {
            _stateStore[internalNode] = value;
        }

        private bool TryAcquireLock(IInternalForestNode internalNode)
        {
            if (_lock != null)
                return false;
            _lock = internalNode;
            return true;
        }

        private bool TryReleaseLock(IInternalForestNode internalNode)
        {
            if (_lock != null && !_lock.Equals(internalNode))
                return false;
            _lock = null;
            return true;
        }

        private bool HasMoreTransitions(IInternalForestNode internalNode)
        {
            return HasMoreTransitions(
                internalNode,
                GetTraversalPosition(internalNode));
        }

        private static bool HasMoreTransitions(IInternalForestNode internalNode, int currentPosition)
        {
            return currentPosition < internalNode.Children.Count - 1;
        }
    }
}