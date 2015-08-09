using Pliant.Grammars;

namespace Pliant.Automata
{
    public interface IDfaTransition
    {
        IDfaState Target { get; }
        ITerminal Terminal { get; }
    }
}