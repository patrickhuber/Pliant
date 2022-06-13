using Pliant.Forest;
using Pliant.Grammars;
using System;
using System.Collections.Generic;

namespace Pliant.Tree
{
    public class InternalTreeNode : IInternalTreeNode
    {
        private IForestDisambiguationAlgorithm _disambiguationAlgorithm;
        private IInternalForestNode _internalNode;
        private List<ITreeNode> _children;

        public int Origin { get { return _internalNode.Origin; } }

        public int Location { get { return _internalNode.Location; } }

        public INonTerminal Symbol { get; private set; }

        public InternalTreeNode(
            IInternalForestNode internalNode,
            IForestDisambiguationAlgorithm stateManager)
        {
            _disambiguationAlgorithm = stateManager;
            _internalNode = internalNode;
            _children = new List<ITreeNode>();
            SetSymbol(_internalNode);
        }

        public InternalTreeNode(
            IInternalForestNode internalNode)
            : this(internalNode, new SelectFirstChildDisambiguationAlgorithm())
        {
        }
        
        private void SetSymbol(IInternalForestNode node)
        {
            switch (node.NodeType)
            {
                case Forest.ForestNodeType.Symbol:
                    Symbol = (node as ISymbolForestNode).Symbol as INonTerminal;
                    break;

                case Forest.ForestNodeType.Intermediate:
                    Symbol = (node as IIntermediateForestNode).DottedRule.Production.LeftHandSide;
                    break;
            }
        }

        public IReadOnlyList<ITreeNode> Children
        {
            get
            {
                if (ShouldLoadChildren())
                {
                    var packedNode = _disambiguationAlgorithm.GetCurrentPackedNode(_internalNode);
                    LazyLoadChildren(packedNode);
                }
                return _children;
            }
        }

        private void LazyLoadChildren(IPackedForestNode packedNode)
        {
            for (int c = 0; c < packedNode.Children.Count; c++)
            {
                var child = packedNode.Children[c];
                switch (child.NodeType)
                {
                    // skip intermediate nodes by enumerating children only
                    case ForestNodeType.Intermediate:
                        var intermediateNode = child as IIntermediateForestNode;
                        var currentPackedNode = _disambiguationAlgorithm.GetCurrentPackedNode(intermediateNode);
                        LazyLoadChildren(currentPackedNode);
                        break;

                    // create a internal tree node for symbol forest nodes
                    case ForestNodeType.Symbol:
                        var symbolNode = child as ISymbolForestNode;
                        _children.Add(new InternalTreeNode(symbolNode, _disambiguationAlgorithm));
                        break;
                        
                    // create a tree token node for token forest nodes
                    case ForestNodeType.Token:
                        _children.Add(new TokenTreeNode(child as ITokenForestNode));
                        break;

                    default:
                        throw new Exception("Unrecognized NodeType");
                }
            }            
        }

        private bool ShouldLoadChildren()
        {
            return _children.Count == 0;
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
            return $"({Symbol}, {Origin}, {Location})";
        }
    }
}