using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant.Nodes
{
    public interface INodeVisitable
    {
        void Accept(INodeVisitor visitor, INodeVisitorStateManager stateManager);
    }
}
