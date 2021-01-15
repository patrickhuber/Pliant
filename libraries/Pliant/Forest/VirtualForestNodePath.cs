using Pliant.Charts;
using Pliant.Utilities;

namespace Pliant.Forest
{
    public class VirtualForestNodePath
    {
        private readonly int _hashCode;
        public ITransitionState TransitionState { get; private set; }
        public IForestNode ForestNode { get; private set; }

        public VirtualForestNodePath(ITransitionState transitionState, IForestNode forestNode)
        {
            TransitionState = transitionState;
            ForestNode = forestNode;
            _hashCode = ComputeHashCode(TransitionState, ForestNode);
        }

        private static int ComputeHashCode(ITransitionState transitionState, IForestNode forestNode)
        {
            return HashCode.Compute(
                transitionState.GetHashCode(), 
                forestNode.GetHashCode());
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (!(obj is VirtualForestNodePath virtualForestNodePath))
                return false;
            return TransitionState.Equals(virtualForestNodePath.TransitionState)
                && ForestNode.Equals(virtualForestNodePath.ForestNode);
        }
    }
}