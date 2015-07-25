using Pliant.Grammars;
using System;
namespace Pliant.Builders
{
    public interface IGrammarBuilder
    {
        IGrammarBuilder Production(string name, Action<IRuleBuilder> rules);
        IGrammarBuilder Ignore(ILexerRule lexerRule);
        IGrammar ToGrammar();
    }
}
