using Pliant.Grammars;

namespace Pliant.Builders
{
    public interface ILexerRuleBuilder
    {
        ILexerRuleBuilder LexerRule(string name, ITerminal terminal);
        ILexerRuleBuilder LexerRule(ILexerRule lexerRule);
    }
}
