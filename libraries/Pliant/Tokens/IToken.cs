namespace Pliant.Tokens
{
    public interface IToken
    {
        string Value { get; }
        int Origin { get; }
        TokenType TokenType { get; }
    }
}
