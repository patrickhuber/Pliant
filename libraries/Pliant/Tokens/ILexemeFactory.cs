using Pliant.Grammars;

namespace Pliant.Tokens
{
    public interface ILexemeFactory
    {
        LexerRuleType LexerRuleType { get; }

        ILexeme Create(ILexerRule lexerRule, int position);

        void Free(ILexeme lexeme);
    }
}