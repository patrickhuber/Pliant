using Pliant.Charts;

namespace Pliant.Forest
{
    public class IntermediateNode : InternalNode, IIntermediateNode
    {
        public IState State { get; private set; }

        public override NodeType NodeType { get { return NodeType.Intermediate; } }

        public IntermediateNode(IState state, int origin, int location)
            : base(origin, location)
        {
            State = state;
        }

        public override string ToString()
        {
            return $"({State}, {Origin}, {Location})";
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}