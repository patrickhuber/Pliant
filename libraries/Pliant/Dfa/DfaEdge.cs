using Pliant.Grammars;

namespace Pliant.Dfa
{
    public class DfaEdge : IDfaEdge
    {
        public IDfaState Target { get; private set; }
        public ITerminal Terminal { get; private set; }

        public DfaEdge(ITerminal terminal, IDfaState target)
        {
            Target = target;
        }
    }
}
