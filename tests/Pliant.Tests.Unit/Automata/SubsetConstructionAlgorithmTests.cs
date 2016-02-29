using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Automata;
using Pliant.Grammars;

namespace Pliant.Tests.Unit.Automata
{
    [TestClass]
    public class SubsetConstructionAlgorithmTests
    {
        [TestMethod]
        public void ShouldConvertSingleCharacterNfaToEquivalentDfa()
        {
            var nfaStart = new NfaState();
            var nfaEnd = new NfaState();
            var nfa = new Nfa(nfaStart, nfaEnd);
            nfaStart.AddTransistion(
                new TerminalNfaTransition(
                    new CharacterTerminal('a'), nfaEnd));

            var dfa = ConvertNfaToDfa(nfa);
        }

        [TestMethod]
        public void ShouldConvertComplexNfaToDfa()
        {
            var a = new CharacterTerminal('a');
            var b = new CharacterTerminal('b');
            var c = new CharacterTerminal('c');

            var states = new NfaState[4];
            for (int i = 0; i < states.Length; i++)
                states[i] = new NfaState();

            states[0].AddTransistion(new TerminalNfaTransition(a, states[1]));
            states[0].AddTransistion(new TerminalNfaTransition(c, states[3]));
            states[1].AddTransistion(new NullNfaTransition(states[0]));
            states[1].AddTransistion(new TerminalNfaTransition(b, states[2]));
            states[2].AddTransistion(new TerminalNfaTransition(a, states[1]));
            states[3].AddTransistion(new TerminalNfaTransition(c, states[2]));
            states[3].AddTransistion(new NullNfaTransition(states[2]));

            var dfa = ConvertNfaToDfa(new Nfa(states[0], states[2]));
        }

        private IDfaState ConvertNfaToDfa(INfa nfa)
        {
            return new SubsetConstructionAlgorithm().Transform(nfa);
        }
    }
}
