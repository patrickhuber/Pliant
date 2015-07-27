using Pliant.Grammars;

namespace Pliant.Automata
{
    public interface IDfaEdge
    {
        IDfaState Target { get; }
        ITerminal Terminal { get; }
    }
}