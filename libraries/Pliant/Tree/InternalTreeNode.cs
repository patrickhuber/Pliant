using Pliant.Forest;
using Pliant.Grammars;
using System;
using System.Collections.Generic;

namespace Pliant.Tree
{
    public class InternalTreeNode : IInternalTreeNode
    {
        private IForestNodeVisitorStateManager _stateManager;
        private IAndForestNode _currentAndNode;
        private IInternalForestNode _internalNode;

        public int Origin { get { return _internalNode.Origin; } }

        public int Location { get { return _internalNode.Location; } }

        public INonTerminal Symbol { get; private set; }

        public InternalTreeNode(
            IInternalForestNode internalNode,
            IAndForestNode currentAndNode,
            IForestNodeVisitorStateManager stateManager)
        {
            _stateManager = stateManager;
            _currentAndNode = currentAndNode;
            _internalNode = internalNode;
            SetRule(_internalNode);
        }

        public InternalTreeNode(
            IInternalForestNode internalNode)
            : this(internalNode, new MultiPassForestNodeVisitorStateManager())
        {
        }

        public InternalTreeNode(
            IInternalForestNode internalNode,
            IForestNodeVisitorStateManager stateManager)
            : this(internalNode, stateManager.GetCurrentAndNode(internalNode), stateManager)
        {
        }

        private void SetRule(IInternalForestNode node)
        {
            switch (node.NodeType)
            {
                case Forest.ForestNodeType.Symbol:
                    Symbol = (node as ISymbolForestNode).Symbol as INonTerminal;
                    break;

                case Forest.ForestNodeType.Intermediate:
                    Symbol = (node as IIntermediateForestNode).State.Production.LeftHandSide;
                    break;
            }
        }

        public IEnumerable<ITreeNode> Children
        {
            get
            {
                return EnumerateChildren(_currentAndNode);
            }
        }

        private IEnumerable<ITreeNode> EnumerateChildren(IAndForestNode andNode)
        {
            foreach (var child in andNode.Children)
            {
                switch (child.NodeType)
                {
                    case Forest.ForestNodeType.Intermediate:
                        var intermediateNode = child as IIntermediateForestNode;
                        var currentAndNode = _stateManager.GetCurrentAndNode(intermediateNode);
                        foreach (var otherChild in EnumerateChildren(currentAndNode))
                            yield return otherChild;
                        break;

                    case Forest.ForestNodeType.Symbol:
                        var symbolNode = child as ISymbolForestNode;
                        var childAndNode = _stateManager.GetCurrentAndNode(symbolNode);
                        yield return new InternalTreeNode(symbolNode, childAndNode, _stateManager);
                        break;

                    case Forest.ForestNodeType.Token:
                        yield return new TokenTreeNode(child as ITokenForestNode);
                        break;

                    default:
                        throw new Exception("Unrecognized NodeType");
                }
            }
        }

        public TreeNodeType NodeType
        {
            get { return TreeNodeType.Internal; }
        }

        public void Accept(ITreeNodeVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override string ToString()
        {
            return $"{Symbol}({Origin}, {Location})";
        }
    }
}