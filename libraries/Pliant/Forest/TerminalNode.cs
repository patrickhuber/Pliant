namespace Pliant.Forest
{
    public class TerminalNode : NodeBase, ITerminalNode
    {
        public char Capture { get; private set; }

        public TerminalNode(char capture, int origin, int location)
            : base(origin, location)
        {
            Capture = capture;
        }

        public override NodeType NodeType
        {
            get { return NodeType.Terminal; }
        }

        public override string ToString()
        {
            return $"({(Capture == '\0' ? "null" : Capture.ToString())}, {Origin}, {Location})";
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override bool Equals(object obj)
        {
            if ((object)obj == null)
                return false;

            var terminalNode = obj as TerminalNode;
            if ((object)terminalNode == null)
                return false;

            return Location == terminalNode.Location
                && NodeType == terminalNode.NodeType
                && Origin == terminalNode.Origin
                && Capture.Equals(terminalNode.Capture);
        }

        private readonly int _hashCode;

        private int ComputeHashCode()
        {
            return HashUtil.ComputeHash(
                NodeType.GetHashCode(),
                Location.GetHashCode(),
                Origin.GetHashCode(),
                Capture.GetHashCode());
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }
    }
}