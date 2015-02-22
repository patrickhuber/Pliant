using System;
namespace Earley
{
    public interface IGrammarBuilder
    {
        IGrammarBuilder Lexeme();
        IGrammarBuilder Production(string name, Action<IRuleBuilder> rules);
    }
}
