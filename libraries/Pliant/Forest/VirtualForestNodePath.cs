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
            if (((object)obj) == null)
                return false;
            var virtualForestNodePath = obj as VirtualForestNodePath;
            if (((object)virtualForestNodePath) == null)
                return false;
            return TransitionState.Equals(virtualForestNodePath.TransitionState)
                && ForestNode.Equals(virtualForestNodePath.ForestNode);
        }
    }
}