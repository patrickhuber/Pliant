using Pliant.Tokens;

namespace Pliant.Forest
{
    public class TokenNode : NodeBase, ITokenNode
    {
        public IToken Token { get; private set; }

        public TokenNode(IToken token, int origin, int location)
            : base(origin, location)
        {
            Token = token;
            _hashCode = ComputeHashCode();
        }

        public override NodeType NodeType
        {
            get { return NodeType.Token; }
        }

        public override void Accept(INodeVisitor visitor)
        {
            visitor.Visit(this);
        }
        
        public override bool Equals(object obj)
        {
            if ((object)obj == null)
                return false;

            var tokenNode = obj as TokenNode;
            if ((object)tokenNode == null)
                return false;

            return Location == tokenNode.Location
                && NodeType == tokenNode.NodeType
                && Origin == tokenNode.Origin
                && Token.Equals(tokenNode.Token);
        }

        private readonly int _hashCode;
        
        private int ComputeHashCode()
        {
            return HashUtil.ComputeHash(
                NodeType.GetHashCode(),
                Location.GetHashCode(),
                Origin.GetHashCode(),
                Token.GetHashCode());
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }
    }
}