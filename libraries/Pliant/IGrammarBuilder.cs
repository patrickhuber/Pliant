using System;
namespace Pliant
{
    public interface IGrammarBuilder
    {
        IGrammarBuilder Start(string name);
        IGrammarBuilder Production(string name, Action<IRuleBuilder> rules);
        IGrammarBuilder Lexeme(string name, Action<ITerminalBuilder> terminals);
        IGrammarBuilder Ignore(string name);
    }
}
