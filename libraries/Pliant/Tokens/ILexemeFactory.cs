using Pliant.Captures;
using Pliant.Grammars;

namespace Pliant.Tokens
{
    public interface ILexemeFactory
    {
        LexerRuleType LexerRuleType { get; }
                
        ILexeme Create(ILexerRule lexerRule, ICapture<char> segment, int offset);

        void Free(ILexeme lexeme);
    }
}