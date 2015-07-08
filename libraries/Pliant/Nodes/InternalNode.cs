using Pliant.Collections;
using System.Collections.Generic;

namespace Pliant.Nodes
{
    public abstract class InternalNode : IInternalNode
    {
        private ReadWriteList<IAndNode> _children;

        public int Origin { get; private set; }
        
        public int Location { get; private set; }

        public IReadOnlyList<IAndNode> Children { get { return _children; } }

        public abstract NodeType NodeType { get; }

        protected InternalNode(int origin, int location)
        {
            Origin = origin;
            Location = location;
            _children = new ReadWriteList<IAndNode>();
        }

        public abstract void Accept(INodeVisitor visitor);

        public void AddUniqueFamily(INode trigger)
        {
            AddUniqueAndNode(trigger);
        }

        public void AddUniqueFamily(INode source, INode trigger)
        {
            AddUniqueAndNode(source, trigger);
        }
        
        private void AddUniqueAndNode(params INode[] children)
        {
            foreach (var andNode in _children)
            {
                if (andNode.Children.Count != children.Length)
                    continue;
                bool isMatchedSubTree = IsMatchedSubTree(children, andNode);
                if (isMatchedSubTree)
                    return;
            }

            // not found so return new and node
            var newAndNode = new AndNode();
            foreach (var child in children)
                newAndNode.AddChild(child);

            _children.Add(newAndNode); 
        }

        private static bool IsMatchedSubTree(INode[] children, IAndNode andNode)
        {
            for (var c = 0; c < andNode.Children.Count; c++)
            {
                var parameterNode = children[c];
                var compareNode = andNode.Children[c];

                if (!IsSameNode(parameterNode, compareNode))
                    return false;
            }
            return true;
        }

        private static bool IsSameNode(INode parameterNode, INode compareNode)
        {
            if (parameterNode.NodeType != compareNode.NodeType)
                return false;

            if (parameterNode.Origin != compareNode.Origin)
                return false;

            if (parameterNode.Location != compareNode.Location)
                return false;

            if (parameterNode.NodeType == NodeType.Symbol)
            {
                var symbolParameterNode = parameterNode as ISymbolNode;
                var symbolCompareNode = compareNode as ISymbolNode;
                return IsSameSymbolNode(symbolParameterNode, symbolCompareNode);
            }
            else if (parameterNode.NodeType == NodeType.Terminal)
            {
                var terminalParameterNode = parameterNode as ITerminalNode;
                var terminalCompareNode = compareNode as ITerminalNode;
                return IsSameTerminalNode(terminalParameterNode, terminalCompareNode);
            }
            
            var intermediateParameterNode = parameterNode as IIntermediateNode;
            var intermediateCompareNode = compareNode as IIntermediateNode;
            return IsSameIntermediateNode(intermediateParameterNode, intermediateCompareNode);
        }

        private static bool IsSameSymbolNode(ISymbolNode symbolParameterNode, ISymbolNode symbolCompareNode)
        {
            return symbolParameterNode.Symbol.Equals(symbolCompareNode.Symbol);
        }

        private static bool IsSameTerminalNode(ITerminalNode terminalParameterNode, ITerminalNode terminalCompareNode)
        {
            return terminalParameterNode.Capture == terminalCompareNode.Capture;
        }

        private static bool IsSameIntermediateNode(IIntermediateNode intermediateParameterNode, IIntermediateNode intermediateCompareNode)
        {
            return intermediateParameterNode.State.Equals(intermediateCompareNode.State);
        }
    }
}
