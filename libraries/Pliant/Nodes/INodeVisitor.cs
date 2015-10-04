using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant.Nodes
{
    public interface INodeVisitor
    {
        void Visit(ITerminalNode node, INodeVisitorStateManager stateManager);
        void Visit(ISymbolNode node, INodeVisitorStateManager stateManager);
        void Visit(IIntermediateNode node, INodeVisitorStateManager stateManager);
        void Visit(IAndNode andNode, INodeVisitorStateManager stateManager);
        void Visit(ITokenNode tokenNode, INodeVisitorStateManager stateManager);
    }
}
