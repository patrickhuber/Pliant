namespace Pliant.Tokens
{
    public class TokenType
    {
        public string Id { get; private set; }

        public TokenType(string id)
        {
            Id = id;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            var other = obj as TokenType;
            if (other == null)
                return false;
            return other.Id.Equals(Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
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
