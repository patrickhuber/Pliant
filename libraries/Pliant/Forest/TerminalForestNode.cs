using Pliant.Utilities;

namespace Pliant.Forest
{
    public class TerminalForestNode : ForestNodeBase, ITerminalForestNode
    {
        public char Capture { get; private set; }

        public TerminalForestNode(char capture, int origin, int location)
            : base(origin, location)
        {
            Capture = capture;
            _hashCode = ComputeHashCode();
        }

        public override ForestNodeType NodeType
        {
            get { return ForestNodeType.Terminal; }
        }

        public override string ToString()
        {
            return $"({(Capture == '\0' ? "null" : Capture.ToString())}, {Origin}, {Location})";
        }

        public override void Accept(IForestNodeVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;

            if (!(obj is TerminalForestNode terminalNode))
                return false;

            return Location == terminalNode.Location
                && NodeType == terminalNode.NodeType
                && Origin == terminalNode.Origin
                && Capture.Equals(terminalNode.Capture);
        }

        private readonly int _hashCode;

        private int ComputeHashCode()
        {
            return HashCode.Compute(
                ((int)NodeType).GetHashCode(),
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