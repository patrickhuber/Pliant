using Pliant.Grammars;

namespace Pliant.Automata
{
    public class TerminalNfaTransition : INfaTransition
    {
        public INfaState Target { get; private set; }

        public ITerminal Terminal { get; private set; }

        public NfaTransitionType TransitionType { get { return NfaTransitionType.Terminal; } }
    }
}
