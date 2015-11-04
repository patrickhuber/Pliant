using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant.Ast
{
    public interface INodeVisitorStateManager
    {
        IAndNode GetCurrentAndNode(IInternalNode internalNode);
        void MarkAsTraversed(IInternalNode internalNode);
    }
}
