using System;
namespace Pliant
{
    public interface IGrammarBuilder
    {
        IGrammarBuilder Production(string name, Action<IRuleBuilder> rules);
        IGrammarBuilder CharacterClass(string name, Action<ITerminalBuilder> terminal);
    }
}
