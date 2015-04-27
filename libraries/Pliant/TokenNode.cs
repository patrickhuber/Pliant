using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant
{
    public class TokenNode : ITokenNode
    {
        public IToken Capture { get; private set; }

        public int Origin { get; private set; }

        public int Location { get; private set; }

        public NodeType NodeType { get { return NodeType.Token; } }


    }
}
