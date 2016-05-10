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
            _hashCode = ComputeHashCode();
        }

        public override string ToString()
        {
            return $"({State}, {Origin}, {Location})";
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }
        public override bool Equals(object obj)
        {
            if ((object)obj == null)
                return false;

            var intermediateNode = obj as IntermediateNode;
            if ((object)intermediateNode == null)
                return false;

            return Location == intermediateNode.Location
                && NodeType == intermediateNode.NodeType
                && Origin == intermediateNode.Origin
                && State.Equals(intermediateNode.State);
        }

        private readonly int _hashCode;

        private int ComputeHashCode()
        {
            return HashUtil.ComputeHash(
                NodeType.GetHashCode(),
                Location.GetHashCode(),
                Origin.GetHashCode(),
                State.GetHashCode());
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }
    }
}