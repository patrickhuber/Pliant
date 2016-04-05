using Pliant.Forest;
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

        public override void Visit(IIntermediateNode intermediateNode)
        {
            VisitLog.Add(intermediateNode.ToString());
            foreach (var child in intermediateNode.Children)
                Visit(child);
        }

        public override void Visit(ITokenNode tokenNode)
        {
            VisitLog.Add(tokenNode.ToString());
        }

        public override void Visit(ISymbolNode symbolNode)
        {
            VisitLog.Add(symbolNode.ToString());
            foreach (var child in symbolNode.Children)
                Visit(child);
        }

        public override void Visit(ITerminalNode terminalNode)
        {
            VisitLog.Add(terminalNode.ToString());
        }

        public override void Visit(IAndNode andNode)
        {
            foreach (var child in andNode.Children)
                child.Accept(this);
        }
    }
}