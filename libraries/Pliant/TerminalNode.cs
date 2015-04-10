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

        public TerminalNode(char capture, int origin, int location)
        {
            Capture = capture;
            Origin = origin;
            Location = location;
        }
    }
}
