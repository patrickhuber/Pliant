namespace Pliant.Tokens
{
    public interface IToken
    {
        string Value { get; }
        int Position { get; }
        TokenType TokenType { get; }
    }
}