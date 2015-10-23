using Pliant.Nodes;
using System;
using System.Collections.Generic;

namespace Pliant.Tests.Unit
{
    public class LoggingNodeVisitor : NodeVisitorBase
    {
        public IList<string> VisitLog { get; private set; }

        public LoggingNodeVisitor()
        {
            Init();
        }

        public LoggingNodeVisitor(INodeVisitorStateManager stateManager)
            : base(stateManager)
        {
            Init();
        }

        private void Init()
        {
            VisitLog = new List<string>();
        }

        public override void Visit(IIntermediateNode node)
        {
            VisitLog.Add(node.ToString());
            foreach (var child in node.Children)
                Visit(child);
        }

        public override void Visit(ITokenNode node)
        {
            VisitLog.Add(node.ToString());
        }

        public override void Visit(ISymbolNode node)
        {
            VisitLog.Add(node.ToString());
            foreach (var child in node.Children)
                Visit(child);
        }

        public override void Visit(ITerminalNode node)
        {
            VisitLog.Add(node.ToString());
        }

        public override void Visit(IAndNode node)
        {            
            foreach (var child in node.Children)
                child.Accept(this);
        }        
    }
}
