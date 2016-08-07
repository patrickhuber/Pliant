using Pliant.Charts;
using Pliant.Forest;

namespace Pliant.Tests.Common.Forest
{
    public class FakeIntermediateForestNode : FakeInternalForestNode, IIntermediateForestNode
    {
        public FakeIntermediateForestNode(IState state, int origin, int location, params IAndForestNode[] children) 
            : base(origin, location, children)
        {
            State = state;
        }

        public override ForestNodeType NodeType
        {
            get { return ForestNodeType.Intermediate; }
        }

        public IState State { get; private set; }
    }
}
