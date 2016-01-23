using Pliant.Grammars;

namespace Pliant.Lexemes
{
    public interface ILexemeFactoryRegistry
    {
        ILexemeFactory Get(LexerRuleType lexerRuleType);

        void Register(ILexemeFactory factory);
    }
}