namespace Pliant.Tokens
{
    public class Token : IToken
    {
        public string Value { get; private set; }

        public int Origin { get; private set; }

        public TokenType TokenType { get; private set; }

        public Token(string value, int origin, TokenType tokenType)
        {
            Value = value;
            Origin = origin;
            TokenType = tokenType;
        }
    }
}