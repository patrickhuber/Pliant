using Pliant.Grammars;

namespace Pliant.Dfa
{
    public interface IDfaEdge
    {
        IDfaState Target { get; }
        ITerminal Terminal { get; }
    }
}