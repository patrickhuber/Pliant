using Pliant.Nodes;
using System;
using System.Collections.Generic;

namespace Pliant.Tests.Unit
{
    public class LoggingNodeVisitor : INodeVisitor
    {
        public IList<string> VisitLog { get; }

        public LoggingNodeVisitor()
        {
            VisitLog = new List<string>();
        }
        
        public void Visit(IIntermediateNode node, INodeVisitorStateManager stateManager)
        {
            VisitLog.Add(node.ToString());
        }

        public void Visit(ITokenNode node, INodeVisitorStateManager stateManager)
        {
            VisitLog.Add(node.ToString());
        }

        public void Visit(ISymbolNode node, INodeVisitorStateManager stateManager)
        {
            VisitLog.Add(node.ToString());
        }

        public void Visit(ITerminalNode node, INodeVisitorStateManager stateManager)
        {
            VisitLog.Add(node.ToString());
        }

        public void Visit(IAndNode node, INodeVisitorStateManager stateManager)
        {            
            foreach (var child in node.Children)
                child.Accept(this, stateManager);
        }        
    }
}
