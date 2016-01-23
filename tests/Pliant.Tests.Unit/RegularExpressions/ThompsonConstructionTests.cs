using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Automata;
using Pliant.Grammars;
using Pliant.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

namespace Pliant.Tests.Unit.RegularExpressions
{
    [TestClass]
    public class ThompsonConstructionTests
    {
        [TestMethod]
        public void ThompsonConstructionShouldCreateNfaFromEmptyString()
        {
            var input = "";
            var nfa = CreateNfa(input);
            VerifyNfaIsNotNullAndHasValidStartAndEndStates(nfa);
            Assert.IsFalse(nfa.End.Transitions.Any());
            var list = new List<INfaTransition>(nfa.Start.Transitions);
            Assert.AreEqual(1, list.Count);
            var nullNfaTransition = nfa.Start.Transitions.First() as NullNfaTransition;
            Assert.AreEqual(nfa.End, nullNfaTransition.Target);
        }
        
        [TestMethod]
        public void ThompsonConstructionShouldCreatNfaFromCharacterExpression()
        {
            var input = "a";
            var nfa = CreateNfa(input);
            VerifyNfaIsNotNullAndHasValidStartAndEndStates(nfa);

            Assert.AreEqual(1, nfa.Start.Transitions.Count());

            var firstTransition = nfa.Start.Transitions.FirstOrDefault();
            VerifyCharacterTransition(firstTransition, 'a');            
        }

        [TestMethod]
        public void ThompsonConstructionShouldCreateNfaFromCharacterClass()
        {
            var input = "[a]";
            var nfa = CreateNfa(input);
            Assert.IsNotNull(nfa);
        }

        [TestMethod]
        public void ThompsonConstructionShouldCreateChangedTransitionStatestFromConcatenation()
        {
            var input = "ab";
            var nfa = CreateNfa(input);
            VerifyNfaIsNotNullAndHasValidStartAndEndStates(nfa);

            Assert.AreEqual(1, nfa.Start.Transitions.Count());

            var firstTransition = nfa.Start.Transitions.FirstOrDefault();
            VerifyCharacterTransition(firstTransition, 'a');
            Assert.AreEqual(1, firstTransition.Target.Transitions.Count());

            var secondTransition = firstTransition.Target.Transitions.FirstOrDefault();
            VerifyNullTransition(secondTransition);
            Assert.AreEqual(1, secondTransition.Target.Transitions.Count());

            var thirdTransition = secondTransition.Target.Transitions.FirstOrDefault();
            VerifyCharacterTransition(thirdTransition, 'b');
            Assert.AreEqual(0, thirdTransition.Target.Transitions.Count());
            Assert.AreSame(nfa.End, thirdTransition.Target);
        }

        [TestMethod]
        public void ThompsonConstructionShouldCreateOptionalPathNfaFromUnionExpression()
        {
            var input = "a|b";
            var nfa = CreateNfa(input);
            VerifyNfaIsNotNullAndHasValidStartAndEndStates(nfa);

            Assert.AreEqual(1, nfa.Start.Transitions.Count());

            var firstTransition = nfa.Start.Transitions.FirstOrDefault();
            VerifyNullTransition(firstTransition);
            Assert.AreEqual(2, firstTransition.Target.Transitions.Count());
        }

        private static void VerifyNfaIsNotNullAndHasValidStartAndEndStates(INfa nfa)
        {
            Assert.IsNotNull(nfa);
            Assert.IsNotNull(nfa.Start);
            Assert.IsNotNull(nfa.End);
            Assert.AreEqual(0, nfa.End.Transitions.Count());
        }

        private static void VerifyCharacterTransition(INfaTransition transition, char character)
        {
            var terminalNfaTransition = transition as TerminalNfaTransition;
            Assert.IsNotNull(terminalNfaTransition);
            var characterTerminal = terminalNfaTransition.Terminal as CharacterTerminal;
            Assert.AreEqual(character, characterTerminal.Character);
        }

        private static void VerifyNullTransition(INfaTransition transition)
        {
            Assert.IsInstanceOfType(transition, typeof(NullNfaTransition));
        }

        private static INfa CreateNfa(string input)
        {
            var regex = new RegexParser().Parse(input);
            return new ThompsonConstructionAlgorithm().Transform(regex);
        }
    }
}
