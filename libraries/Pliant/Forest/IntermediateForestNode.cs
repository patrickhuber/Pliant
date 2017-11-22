using Pliant.Charts;
using Pliant.Grammars;
using Pliant.Utilities;

namespace Pliant.Forest
{
    public class IntermediateForestNode : InternalForestNode, IIntermediateForestNode
    {
        public IDottedRule DottedRule { get; private set; }

        public override ForestNodeType NodeType { get { return ForestNodeType.Intermediate; } }

        public IntermediateForestNode(IDottedRule dottedRule, int origin, int location)
            : base(origin, location)
        {
            DottedRule = dottedRule;
            _hashCode = ComputeHashCode();
        }

        public override string ToString()
        {
            return $"({DottedRule}, {Origin}, {Location})";
        }

        public override void Accept(IForestNodeVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            var intermediateNode = obj as IIntermediateForestNode;
            if (intermediateNode == null)
                return false;

            return Location == intermediateNode.Location
                && NodeType == intermediateNode.NodeType
                && Origin == intermediateNode.Origin
                && DottedRule.Equals(intermediateNode.DottedRule);
        }

        private readonly int _hashCode;

        private int ComputeHashCode()
        {
            return HashCode.Compute(
                ((int)NodeType).GetHashCode(),
                Location.GetHashCode(),
                Origin.GetHashCode(),
                DottedRule.GetHashCode());
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }
    }
}