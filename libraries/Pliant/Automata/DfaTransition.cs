using Pliant.Grammars;

namespace Pliant.Automata
{
    public class DfaTransition : IDfaTransition
    {
        public IDfaState Target { get; private set; }
        public ITerminal Terminal { get; private set; }

        public DfaTransition(ITerminal terminal, IDfaState target)
        {
            Target = target;
            Terminal = terminal;
        }
    }
}