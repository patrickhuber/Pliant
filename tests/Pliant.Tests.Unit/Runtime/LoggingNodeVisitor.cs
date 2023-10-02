using Pliant.Forest;
using System.Collections.Generic;

namespace Pliant.Tests.Unit.Runtime
{
    public class LoggingNodeVisitor : DisambiguatingForestNodeVisitorBase
    {
        public IList<string> VisitLog { get; private set; }
        
        public LoggingNodeVisitor(IForestDisambiguationAlgorithm stateManager)
            : base(stateManager)
        {
            Init();
        }

        private void Init()
        {
            VisitLog = new List<string>();
        }

        public override void Visit(IIntermediateForestNode intermediateNode)
        {
            VisitLog.Add(intermediateNode.ToString());
        }

        public override void Visit(ITokenForestNode tokenNode)
        {
            VisitLog.Add(tokenNode.ToString());
        }

        public override void Visit(ISymbolForestNode symbolNode)
        {
            VisitLog.Add(symbolNode.ToString());
        }

        public override void Visit(ITerminalForestNode terminalNode)
        {
            VisitLog.Add(terminalNode.ToString());
        }

        public override void Visit(IPackedForestNode packedNode)
        {
            for (int i = 0; i < packedNode.Children.Count; i++)
            {
                IForestNode child = packedNode.Children[i];
                child.Accept(this);
            }
        }
    }
}