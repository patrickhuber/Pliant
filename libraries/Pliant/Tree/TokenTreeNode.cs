using Pliant.Tokens;
using Pliant.Ast;
using System;

namespace Pliant.Tree
{
    public class TokenTreeNode : ITokenTreeNode
    {
        private ITokenNode _innerNode;

        public int Origin { get { return _innerNode.Origin; } }
        public int Location { get { return _innerNode.Location; } }

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

        public override string ToString()
        {
            return $"{Token.TokenType.Id}({Origin}, {Location}) = {Token.Value}";
        }
    }
}
