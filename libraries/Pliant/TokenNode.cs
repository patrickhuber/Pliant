namespace Pliant
{
    public class TokenNode : ITokenNode
    {
        public IToken Token { get; private set; }

        public int Origin { get; private set; }

        public int Location { get; private set; }

        public NodeType NodeType { get { return NodeType.Token; } }

        public TokenNode(IToken token, int origin, int location)
        {
            Token = token;
            Origin = origin;
            Location = location;
        }
    }
}
