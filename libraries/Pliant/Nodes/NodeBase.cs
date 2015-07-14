using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant.Nodes
{
    public abstract class NodeBase : INode
    {
        public virtual int Location { get; protected set; }

        public virtual NodeType NodeType { get; protected set; }

        public virtual int Origin { get; protected set; }

        public virtual void Accept(INodeVisitor visitor)
        {
            // don't visit intermediate nodes, just their children
            if (NodeType != Nodes.NodeType.Intermediate)            
                Visit(visitor, this);

            if (NodeType == Nodes.NodeType.Symbol ||
                NodeType == Nodes.NodeType.Intermediate)
            { 
                var internalNode = this as IInternalNode;
                var firstAndNode = internalNode.Children[0];
                foreach (var child in firstAndNode.Children)
                    child.Accept(visitor);
            }
        }

        private void Visit(INodeVisitor visitor, INode node)
        {
            switch (node.NodeType)
            {
                case Nodes.NodeType.Intermediate:
                    visitor.Visit(node as IIntermediateNode);
                    break;
                case Nodes.NodeType.Symbol:
                    visitor.Visit(node as ISymbolNode);
                    break;
                case Nodes.NodeType.Terminal:
                    visitor.Visit(node as ITerminalNode);
                    break;
                case Nodes.NodeType.Token:
                    visitor.Visit(node as ITokenNode);
                    break;
            }
        }
    }
}
