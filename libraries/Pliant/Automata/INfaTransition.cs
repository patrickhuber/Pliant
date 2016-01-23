namespace Pliant.Automata
{
    public interface INfaTransition
    {
        INfaState Target { get; }
        NfaTransitionType TransitionType { get; }
    }
}