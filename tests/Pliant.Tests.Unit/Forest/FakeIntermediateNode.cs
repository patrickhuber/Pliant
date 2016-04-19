using Pliant.Charts;
using Pliant.Forest;

namespace Pliant.Tests.Unit.Forest
{
    public class FakeIntermediateNode : FakeInternalNode, IIntermediateNode
    {
        public FakeIntermediateNode(IState state, int origin, int location, params IAndNode[] children) 
            : base(origin, location, children)
        {
            State = state;
        }

        public override NodeType NodeType
        {
            get { return NodeType.Intermediate; }
        }

        public IState State { get; private set; }
    }
}
