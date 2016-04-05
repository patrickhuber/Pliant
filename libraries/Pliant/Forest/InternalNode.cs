using Pliant.Collections;
using System;
using System.Collections.Generic;

namespace Pliant.Forest
{
    public abstract class InternalNode : NodeBase, IInternalNode
    {
        private ReadWriteList<IAndNode> _children;

        public IReadOnlyList<IAndNode> Children { get { return _children; } }

        protected InternalNode(int origin, int location)
        {
            Origin = origin;
            Location = location;
            _children = new ReadWriteList<IAndNode>();
        }

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
                var isMatchedSubTree = IsMatchedSubTree(children, andNode);
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

            switch (parameterNode.NodeType)
            {
                case NodeType.Symbol:
                    return IsSameNode(parameterNode as ISymbolNode, compareNode as ISymbolNode);

                case NodeType.Terminal:
                    return IsSameNode(parameterNode as ITerminalNode, compareNode as ITerminalNode);

                case NodeType.Token:
                    return IsSameNode(parameterNode as ITokenNode, compareNode as ITokenNode);

                case NodeType.Intermediate:
                    return IsSameNode(parameterNode as IIntermediateNode, compareNode as IIntermediateNode);
            }
            throw new InvalidOperationException(
                $"Unrecognized node type {parameterNode.NodeType} found when comparing nodes.");
        }

        private static bool IsSameNode(ISymbolNode symbolParameterNode, ISymbolNode symbolCompareNode)
        {
            return symbolParameterNode.Symbol.Equals(symbolCompareNode.Symbol);
        }

        private static bool IsSameNode(ITerminalNode terminalParameterNode, ITerminalNode terminalCompareNode)
        {
            return terminalParameterNode.Capture == terminalCompareNode.Capture;
        }

        private static bool IsSameNode(IIntermediateNode intermediateParameterNode, IIntermediateNode intermediateCompareNode)
        {
            return intermediateParameterNode.State.Equals(intermediateCompareNode.State);
        }

        private static bool IsSameNode(ITokenNode tokenparameterNode, ITokenNode tokenCompareNode)
        {
            return tokenparameterNode.Token.TokenType.Equals(tokenCompareNode.Token.TokenType);
        }
    }
}