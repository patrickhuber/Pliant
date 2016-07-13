using Pliant.Utilities;

namespace Pliant.Forest
{
    public abstract class ForestNodeBase : IForestNode
    {
        private readonly int _hashCode;
        protected ForestNodeBase(int origin, int location)
        {
            Origin = origin;
            Location = location;
            _hashCode = ComputeHashCode();
        }

        public int Location { get; private set; }

        public abstract ForestNodeType NodeType { get; }

        public int Origin { get; private set; }

        public abstract void Accept(IForestNodeVisitor visitor);

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            var nodeBase = obj as ForestNodeBase;
            if (nodeBase == null)
                return false;

            return Location == nodeBase.Location
                && NodeType == nodeBase.NodeType
                && Origin == nodeBase.Origin;
        }
        public override int GetHashCode()
        {
            return _hashCode;
        }

        private int ComputeHashCode()
        {
            return HashCode.Compute(
                ((int)NodeType).GetHashCode(),
                Location.GetHashCode(),
                Origin.GetHashCode());
        }
    }
}