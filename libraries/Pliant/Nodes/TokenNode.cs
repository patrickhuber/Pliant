using System;
using Pliant.Tokens;

namespace Pliant.Nodes
{
    public class TokenNode : NodeBase, ITokenNode
    {
        public IToken Token { get; private set; }
        
        public TokenNode(IToken token, int origin, int location)
        {
            Token = token;
            Origin = origin;
            Location = location;
            NodeType = NodeType.Token;
        }

        public override void Accept(INodeVisitor visitor, INodeVisitorStateManager stateManager)
        {
            visitor.Visit(this, stateManager);
        }
    }
}
