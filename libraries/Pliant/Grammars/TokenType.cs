namespace Pliant.Grammars
{
    public class TokenType
    {
        public string Id { get; private set; }
        private readonly int _hashCode;
        public TokenType(string id)
        {
            Id = id;
            _hashCode = ComputeHashCode(Id);
        }

        private static int ComputeHashCode(string id)
        {
            return id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (!(obj is TokenType other))
                return false;
            return other.Id.Equals(Id);
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }

        public override string ToString()
        {
            return Id;
        }

        public static bool operator ==(TokenType first, TokenType second)
        {
            return first.Equals(second);
        }

        public static bool operator !=(TokenType first, TokenType second)
        {
            return !(first == second);
        }
    }
}