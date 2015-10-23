using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant.Nodes
{
    public abstract class NodeVisitorBase : INodeVisitor
    {
        public INodeVisitorStateManager StateManager { get; private set; }

        protected NodeVisitorBase(INodeVisitorStateManager stateManager)
        {
            StateManager = stateManager;
        }

        protected NodeVisitorBase()
            : this(new NodeVisitorStateManager())
        {
        }

        public abstract void Visit(IIntermediateNode node);

        public abstract void Visit(ITokenNode tokenNode);

        public abstract void Visit(IAndNode andNode);

        public abstract void Visit(ISymbolNode node);

        public abstract void Visit(ITerminalNode node);
    }
}
