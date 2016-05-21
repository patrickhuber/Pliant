namespace Pliant.Forest
{
    public abstract class ForestNodeBase : IForestNode
    {
        public int Location { get; private set; }

        public abstract ForestNodeType NodeType { get; }

        public int Origin { get; private set; }

        public abstract void Accept(IForestNodeVisitor visitor);

        protected ForestNodeBase(int origin, int location)
        {
            Origin = origin;
            Location = location;
            _hashCode = ComputeHashCode();
        }

        public override bool Equals(object obj)
        {
            if ((object)obj == null)
                return false;

            var nodeBase = obj as ForestNodeBase;
            if ((object)nodeBase == null)
                return false;

            return Location == nodeBase.Location
                && NodeType == nodeBase.NodeType
                && Origin == nodeBase.Origin;
        }

        private readonly int _hashCode;

        private int ComputeHashCode()
        {
            return HashUtil.ComputeHash(
                NodeType.GetHashCode(), 
                Location.GetHashCode(), 
                Origin.GetHashCode());
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }
    }
}