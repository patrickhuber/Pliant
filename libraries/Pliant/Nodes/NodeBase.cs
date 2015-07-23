namespace Pliant.Nodes
{
    public abstract class NodeBase : INode
    {
        public virtual int Location { get; protected set; }

        public virtual NodeType NodeType { get; protected set; }

        public virtual int Origin { get; protected set; }

        public virtual void Accept(INodeVisitor visitor, INodeVisitorStateManager stateManager)
        {
            // don't visit intermediate nodes, just their children
            if (NodeType != NodeType.Intermediate)            
                Visit(visitor, this);

            if (NodeType == NodeType.Symbol ||
                NodeType == NodeType.Intermediate)
            { 
                var internalNode = this as IInternalNode;
                var firstAndNode = internalNode.Children[0];
                foreach (var child in firstAndNode.Children)
                    child.Accept(visitor, stateManager);
            }
        }

        private void Visit(INodeVisitor visitor, INode node)
        {
            switch (node.NodeType)
            {
                case NodeType.Intermediate:
                    visitor.Visit(node as IIntermediateNode);
                    break;
                case NodeType.Symbol:
                    visitor.Visit(node as ISymbolNode);
                    break;
                case NodeType.Terminal:
                    visitor.Visit(node as ITerminalNode);
                    break;
                case NodeType.Token:
                    visitor.Visit(node as ITokenNode);
                    break;
            }
        }
    }
}
