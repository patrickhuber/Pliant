using Pliant.Ast;
using System;
using System.Collections.Generic;

namespace Pliant.Tree
{
    public class InternalTreeNode : IInternalTreeNode
    {
        INodeVisitorStateManager _stateManager;
        IAndNode _currentAndNode;
        IInternalNode _internalNode;
        
        public InternalTreeNode(
            IInternalNode internalNode,
            IAndNode currentAndNode,
            INodeVisitorStateManager stateManager)
        {
            _stateManager = stateManager;
            _currentAndNode = currentAndNode;
            _stateManager = stateManager;
        }

        public IEnumerable<ITreeNode> Children
        {
            get
            {
                return EnumerateChildren(_currentAndNode);                        
            }
        }

        private IEnumerable<ITreeNode> EnumerateChildren(IAndNode andNode)
        {
            foreach (var child in andNode.Children)
            {
                switch (child.NodeType)
                {
                    case Ast.NodeType.Intermediate:
                        var intermediateNode = child as IIntermediateNode;
                        var currentAndNode = _stateManager.GetCurrentAndNode(intermediateNode);
                        foreach(var otherChild in EnumerateChildren(currentAndNode))
                            yield return otherChild;
                        break;

                    case Ast.NodeType.Symbol:
                        var symbolNode = child as ISymbolNode;
                        var childAndNode = _stateManager.GetCurrentAndNode(symbolNode);
                        yield return new InternalTreeNode(symbolNode, childAndNode, _stateManager);
                        break;

                    case Ast.NodeType.Token:
                        yield return new TokenTreeNode(child as ITokenNode);
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
    }
}
