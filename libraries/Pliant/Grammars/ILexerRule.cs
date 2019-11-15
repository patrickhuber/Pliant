using Pliant.Tokens;

namespace Pliant.Grammars
{
    public interface ILexerRule : ISymbol
    {
        LexerRuleType LexerRuleType { get; }
        // TODO: eliminate circular reference on Tokens namespace
        TokenType TokenType { get; }
        bool CanApply(char c);
    }
}