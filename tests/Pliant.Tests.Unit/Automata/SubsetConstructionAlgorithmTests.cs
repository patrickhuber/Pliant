using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Automata;
using Pliant.Grammars;

namespace Pliant.Tests.Unit.Automata
{
    [TestClass]
    public class SubsetConstructionAlgorithmTests
    {
        [TestMethod]
        public void SubsetConstructionAlgorithmShouldConvertSingleCharacterNfaToEquivalentDfa()
        {
            var a = new CharacterTerminal('a');
            var states = CreateStates(2);

            states[0].AddTransistion(new TerminalNfaTransition(a, states[1]));

            var nfa = new Nfa(states[0], states[1]);
            var dfa = ConvertNfaToDfa(nfa);

            Assert.IsNotNull(dfa);
        }

        [TestMethod]
        public void SubsetConstructionAlgorithmShouldConvertComplexNfaToDfa()
        {
            var a = new CharacterTerminal('a');
            var b = new CharacterTerminal('b');
            var c = new CharacterTerminal('c');
            var states = CreateStates(4);

            states[0].AddTransistion(new TerminalNfaTransition(a, states[1]));
            states[0].AddTransistion(new TerminalNfaTransition(c, states[3]));
            states[1].AddTransistion(new NullNfaTransition(states[0]));
            states[1].AddTransistion(new TerminalNfaTransition(b, states[2]));
            states[2].AddTransistion(new TerminalNfaTransition(a, states[1]));
            states[3].AddTransistion(new TerminalNfaTransition(c, states[2]));
            states[3].AddTransistion(new NullNfaTransition(states[2]));

            var dfa_0 = ConvertNfaToDfa(new Nfa(states[0], states[2]));

            Assert.IsNotNull(dfa_0);

            IDfaTransition transition_0_01 = null;
            IDfaTransition transition_0_23 = null;

            var count = 0;
            foreach (var transition in dfa_0.Transitions)
            {
                var terminal = transition.Terminal;
                if (terminal.IsMatch('a'))
                    transition_0_01 = transition;
                else if (terminal.IsMatch('c'))
                    transition_0_23 = transition;
                count++;
            }

            Assert.AreEqual(2, count);

            var dfa_01 = transition_0_01.Target;
            IDfaTransition transition_01_01 = null;
            IDfaTransition transition_01_23 = null;
            IDfaTransition transition_01_2 = null;
            
            count = 0;
            foreach (var transition in dfa_01.Transitions)
            {
                var terminal = transition.Terminal;
                if (terminal.IsMatch('a'))
                    transition_01_01 = transition;
                else if (terminal.IsMatch('b'))
                    transition_01_2 = transition;
                else if (terminal.IsMatch('c'))
                    transition_01_23 = transition;
                count++;
            }

            Assert.AreEqual(3, count);

            var dfa_23 = transition_0_23.Target;
            IDfaTransition transition_23_01 = null;
            IDfaTransition transition_23_2 = null;
            
            count = 0;
            foreach (var transition in dfa_23.Transitions)
            {
                var terminal = transition.Terminal;
                if (terminal.IsMatch('a'))
                    transition_23_01 = transition;
                else if (terminal.IsMatch('c'))
                    transition_23_2 = transition;
                count++;
            }

            Assert.AreEqual(2, count);

            var dfa_2 = transition_23_2.Target;
            IDfaTransition transition_2_01 = null;
            count = 0;
            foreach (var transition in dfa_2.Transitions)
            {
                var terminal = transition.Terminal;
                if (terminal.IsMatch('a'))
                    transition_2_01 = transition;
                count++;
            }

            Assert.AreEqual(1, count);
        }

        private static NfaState[] CreateStates(int count)
        {
            var states = new NfaState[count];
            for (int i = 0; i < states.Length; i++)
                states[i] = new EntityNfaState(i);
            return states;
        }

        private IDfaState ConvertNfaToDfa(INfa nfa)
        {
            return new SubsetConstructionAlgorithm().Transform(nfa);
        }

        private class EntityNfaState : NfaState
        {
            public int Id { get; private set; }

            public EntityNfaState(int id)
            {
                Id = id;
            }

            public override int GetHashCode()
            {
                return Id.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                if ((object)obj == null)
                    return false;
                var entityNfaState = obj as EntityNfaState;
                if ((object)entityNfaState == null)
                    return false;
                return entityNfaState.Id.Equals(this.Id);
            }
            
            public override string ToString()
            {
                return "NfaState: " + Id.ToString();
            }
        }
    }
}
