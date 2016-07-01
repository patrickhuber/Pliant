using Pliant.Charts;
using Pliant.Utilities;

namespace Pliant.Forest
{
    public class IntermediateForestNode : InternalForestNode, IIntermediateForestNode
    {
        public IState State { get; private set; }

        public override ForestNodeType NodeType { get { return ForestNodeType.Intermediate; } }

        public IntermediateForestNode(IState state, int origin, int location)
            : base(origin, location)
        {
            State = state;
            _hashCode = ComputeHashCode();
        }

        public override string ToString()
        {
            return $"({State}, {Origin}, {Location})";
        }

        public override void Accept(IForestNodeVisitor visitor)
        {
            visitor.Visit(this);
        }
        public override bool Equals(object obj)
        {
            if ((object)obj == null)
                return false;

            var intermediateNode = obj as IntermediateForestNode;
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
            return HashCode.Compute(
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