using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant
{
    public class TerminalNode : ITerminalNode
    {
        public char Capture { get; private set; }
        
        public int Location { get; private set; }

        public int Origin { get; private set; }

        public NodeType NodeType { get { return NodeType.Terminal; } }

        public TerminalNode(char capture, int origin, int location)
        {
            Capture = capture;
            Origin = origin;
            Location = location;
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
    }
}
