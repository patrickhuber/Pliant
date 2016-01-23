namespace Pliant.Ast
{
    public class TerminalNode : NodeBase, ITerminalNode
    {
        public char Capture { get; private set; }

        public TerminalNode(char capture, int origin, int location)
        {
            Capture = capture;
            Origin = origin;
            Location = location;
            NodeType = NodeType.Terminal;
        }

        public override string ToString()
        {
            return string.Format("({0}, {1}, {2})",
                Capture == '\0'
                ? "null"
                : Capture.ToString(),
                Origin,
                Location);
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}