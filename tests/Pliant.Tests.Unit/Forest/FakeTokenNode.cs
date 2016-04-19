using Pliant.Forest;
using System;
using Pliant.Tokens;

namespace Pliant.Tests.Unit.Forest
{
    public class FakeTokenNode : ITokenNode
    {
        public FakeTokenNode(string token, int origin, int location)
            : this(new Token(token, origin, new TokenType(token)), origin, location)
        {
        }

        public FakeTokenNode(IToken token, int origin, int location)
        {
            Token = token;
            Origin = origin;
            Location = location;
        }

        public int Location { get; private set; }

        public NodeType NodeType
        {
            get { return NodeType.Token; }
        }

        public int Origin { get; private set; }

        public IToken Token { get; private set; }

        public void Accept(INodeVisitor visitor)
        {
            throw new NotImplementedException();
        }
    }
}
