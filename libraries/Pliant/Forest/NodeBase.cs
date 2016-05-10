namespace Pliant.Forest
{
    public abstract class NodeBase : INode
    {
        public int Location { get; private set; }

        public abstract NodeType NodeType { get; }

        public int Origin { get; private set; }

        public abstract void Accept(INodeVisitor visitor);

        protected NodeBase(int origin, int location)
        {
            Origin = origin;
            Location = location;
            _hashCode = ComputeHashCode();
        }

        public override bool Equals(object obj)
        {
            if ((object)obj == null)
                return false;

            var nodeBase = obj as NodeBase;
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