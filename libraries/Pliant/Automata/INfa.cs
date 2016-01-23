namespace Pliant.Automata
{
    public interface INfa
    {
        INfaState Start { get; }
        INfaState End { get; }

        INfa Kleene();

        INfa Concatenation(INfa nfa);

        INfa Union(INfa nfa);
    }
}