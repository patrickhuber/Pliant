namespace Pliant.Automata
{
    public interface INfaToDfa
    {
        IDfaState Transform(INfa nfa);
    }
}