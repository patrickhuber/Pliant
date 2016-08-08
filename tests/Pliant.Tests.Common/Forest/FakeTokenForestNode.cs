using Pliant.Forest;
using System;
using Pliant.Tokens;

namespace Pliant.Tests.Common.Forest
{
    public class FakeTokenForestNode : ITokenForestNode
    {
        public FakeTokenForestNode(string token, int origin, int location)
            : this(new Token(token, origin, new TokenType(token)), origin, location)
        {
        }

        public FakeTokenForestNode(IToken token, int origin, int location)
        {
            Token = token;
            Origin = origin;
            Location = location;
        }

        public int Location { get; private set; }

        public ForestNodeType NodeType
        {
            get { return ForestNodeType.Token; }
        }

        public int Origin { get; private set; }

        public IToken Token { get; private set; }

        public void Accept(IForestNodeVisitor visitor)
        {
            throw new NotImplementedException();
        }
    }
}
