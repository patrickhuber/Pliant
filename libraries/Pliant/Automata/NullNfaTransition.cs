namespace Pliant.Automata
{
    public class NullNfaTransition : INfaTransition
    {
        public INfaState Target { get; private set; }

        public NfaTransitionType TransitionType
        {
            get { return NfaTransitionType.Null; }
        }

        public NullNfaTransition(INfaState target)
        {
            Target = target;
        }
    }
}