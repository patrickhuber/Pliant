using Pliant.Grammars;

namespace Pliant.Tokens
{
    public interface ILexemeFactoryRegistry
    {
        ILexemeFactory Get(LexerRuleType lexerRuleType);

        void Register(ILexemeFactory factory);
    }
}