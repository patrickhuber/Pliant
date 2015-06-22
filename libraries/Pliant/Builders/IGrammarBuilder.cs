using Pliant.Grammars;
using System;
namespace Pliant.Builders
{
    public interface IGrammarBuilder
    {
        IGrammarBuilder Production(string name, Action<IRuleBuilder> rules);
        IGrammarBuilder LexerRule(string name, ITerminal terminal);
        IGrammarBuilder LexerRule(ILexerRule lexerRule);
        IGrammarBuilder Ignore(ILexerRule lexerRule);
        IGrammar ToGrammar();
    }
}
