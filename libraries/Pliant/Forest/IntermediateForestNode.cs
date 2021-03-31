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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0601:Value type to reference type conversion causing boxing allocation", Justification = "ToString is not called in performance critical code")]
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
            if (obj is null)
                return false;

            if (!(obj is IIntermediateForestNode intermediateNode))
                return false;

            return Location == intermediateNode.Location                
                && Origin == intermediateNode.Origin
                && NodeType == intermediateNode.NodeType
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