using Pliant.Automata;
using Pliant.Grammars;
using System;

namespace Pliant.RegularExpressions
{
    public class ThompsonConstructionAlgorithm : IRegexToNfa
    {
        public INfa Transform(Regex regex)
        {
            throw new NotImplementedException();
        }

        private INfa Character(RegexCharacterClassCharacter character)
        {
            var start = new NfaState();
            var end = new NfaState();
            var terminal = new CharacterTerminal(character.Value);
            var transition = new TerminalNfaTransition(
                terminal: terminal,
                target: end);
            start.AddTransistion(transition);
            return new Nfa(start, end);
        }

        private INfa Character(RegexCharacter character)
        {
            var start = new NfaState();
            var end = new NfaState();
            var terminal = new CharacterTerminal(character.Value);
            var transition = new TerminalNfaTransition(
                terminal: terminal,
                target: end);
            start.AddTransistion(transition);
            return new Nfa(start, end);
        }

        private INfa Empty()
        {
            var start = new NfaState();
            var end = new NfaState();
            start.AddTransistion(new NullNfaTransition(end));
            return new Nfa(start, end);
        }

        private INfa Union(INfa first, INfa second)
        {
            var start = new NfaState();
            start.AddTransistion(new NullNfaTransition(first.Start));
            start.AddTransistion(new NullNfaTransition(second.Start));

            var end = new NfaState();
            var endTransition = new NullNfaTransition(end);
            first.End.AddTransistion(endTransition);
            second.End.AddTransistion(endTransition);

            return new Nfa(start, end);
        }

        private INfa Concatenation(INfa first, INfa second)
        {
            first.End.AddTransistion(new NullNfaTransition(second.Start));
            return first;
        }

        private INfa KleeneStar(INfa nfa)
        {
            var start = new NfaState();
            var nullToNfaStart = new NullNfaTransition(nfa.Start);

            start.AddTransistion(nullToNfaStart);
            nfa.End.AddTransistion(nullToNfaStart);

            var end = new NfaState();
            var nullToNewEnd = new NullNfaTransition(end);

            start.AddTransistion(nullToNewEnd);
            nfa.End.AddTransistion(nullToNewEnd);

            return new Nfa(start, end);
        }
    }
}