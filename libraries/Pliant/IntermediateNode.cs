using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant
{
    public class IntermediateNode : InternalNode, IIntermediateNode
    {
        public IState State { get; private set; }

        public override NodeType NodeType { get { return NodeType.Intermediate; } }

        public IntermediateNode(IState state, int origin, int location)
            : base(origin, location)
        {
            State = state;
        }
    }
}
