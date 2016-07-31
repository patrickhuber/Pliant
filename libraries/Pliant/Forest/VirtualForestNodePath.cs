using Pliant.Charts;

namespace Pliant.Forest
{
    public class VirtualForestNodePath
    {
        public ITransitionState TransitionState { get; private set; }
        public IForestNode ForestNode { get; private set; }

        public VirtualForestNodePath(ITransitionState transitionState, IForestNode forestNode)
        {
            TransitionState = transitionState;
            ForestNode = forestNode;
        }
    }
}