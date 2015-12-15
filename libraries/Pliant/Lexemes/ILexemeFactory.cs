using Pliant.Grammars;

namespace Pliant.Lexemes
{
    public interface ILexemeFactory
    {
        LexerRuleType LexerRuleType { get; }

        ILexeme Create(ILexerRule lexerRule);
    }
}