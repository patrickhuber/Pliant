using System;
namespace Earley
{
    public interface IGrammarBuilder
    {
        IGrammarBuilder Production(string name, Action<IRuleBuilder> rules);
        IGrammarBuilder CharacterClass(string name, Action<ITerminalBuilder> terminal);
    }
}
