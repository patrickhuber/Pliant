using Pliant.Tokens;

namespace Pliant.Grammars
{
    public interface ILexerRule : ISymbol
    {
        LexerRuleType LexerRuleType { get; }
        TokenType TokenType { get; }
        bool CanApply(char c);
    }
}