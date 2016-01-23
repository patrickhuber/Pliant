namespace Pliant.Automata
{
    public class Nfa : INfa
    {
        public INfaState End { get; private set; }

        public INfaState Start { get; private set; }

        public Nfa(INfaState start, INfaState end)
        {
            Start = start;
            End = end;
        }

        public INfa Concatenation(INfa nfa)
        {
            End.AddTransistion(
                new NullNfaTransition(nfa.Start));
            return this;
        }

        public INfa Kleene()
        {
            var newStart = new NfaState();
            var newEnd = new NfaState();

            newStart.AddTransistion(new NullNfaTransition(Start));
            newStart.AddTransistion(new NullNfaTransition(newEnd));

            newEnd.AddTransistion(new NullNfaTransition(Start));
            End.AddTransistion(new NullNfaTransition(newEnd));

            return new Nfa(newStart, newEnd);
        }

        public INfa Union(INfa nfa)
        {
            var newStart = new NfaState();
            var newEnd = new NfaState();

            newStart.AddTransistion(new NullNfaTransition(Start));
            newStart.AddTransistion(new NullNfaTransition(nfa.Start));

            End.AddTransistion(new NullNfaTransition(newEnd));
            nfa.End.AddTransistion(new NullNfaTransition(newEnd));

            return new Nfa(newStart, newEnd);
        }
    }
}