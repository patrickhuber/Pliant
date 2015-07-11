using System;
using System.Collections.Generic;

namespace Pliant.Nodes
{
    public class NodeVisitor : INodeVisitor
    {
        public IList<string> VisitLog { get; }

        public NodeVisitor()
        {
            VisitLog = new List<string>();
        }
        
        public void Visit(IIntermediateNode node)
        {
            VisitLog.Add(node.ToString());
        }

        public void Visit(ITokenNode node)
        {
            VisitLog.Add(node.ToString());
        }

        public void Visit(ISymbolNode node)
        {
            VisitLog.Add(node.ToString());
        }

        public void Visit(ITerminalNode node)
        {
            VisitLog.Add(node.ToString());
        }

    }
}
