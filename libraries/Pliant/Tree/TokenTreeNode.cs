using Pliant.Tokens;
using Pliant.Ast;
using System;

namespace Pliant.Tree
{
    public class TokenTreeNode : ITokenTreeNode
    {
        private ITokenNode _innerNode;

        public TokenTreeNode(ITokenNode innerNode)
        {
            _innerNode = innerNode;
        }

        public TreeNodeType NodeType
        {
            get { return TreeNodeType.Token; }
        }

        public IToken Token { get { return _innerNode.Token; } }

        public void Accept(ITreeNodeVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
