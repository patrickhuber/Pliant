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

            /*          / null - q0 - a - q2 - null \
             *      start                           end
             *          \ null - q1 - b - q3 - null /
             *              
             */
            var start = nfa.Start;           
            Assert.AreEqual(2, start.Transitions.Count());

            var startTransitions = new List<INfaTransition>(start.Transitions);
            var firstTransition = startTransitions[0];
            var q0 = firstTransition.Target;
            VerifyNullTransition(firstTransition);
            Assert.AreEqual(1, q0.Transitions.Count());

            var aTransition = q0.Transitions.FirstOrDefault();
            VerifyCharacterTransition(aTransition, 'a');

            var secondTransition = startTransitions[1];
            var q1 = secondTransition.Target;
            VerifyNullTransition(secondTransition);
            Assert.AreEqual(1, q1.Transitions.Count());

            var bTransition = q1.Transitions.FirstOrDefault();
            VerifyCharacterTransition(bTransition, 'b');

            var q2 = aTransition.Target;
            Assert.AreEqual(1, q2.Transitions.Count());
            var q2Transition = q2.Transitions.FirstOrDefault();
            VerifyNullTransition(q2Transition);

            var q3 = bTransition.Target;
            Assert.AreEqual(1, q3.Transitions.Count());
            var q3Transition = q3.Transitions.FirstOrDefault();
            VerifyNullTransition(q3Transition);

            Assert.AreEqual(q2Transition.Target, q3Transition.Target);
        }

        [TestMethod]
        public void ThompsonConstructionShouldRecognizeWhitespaceShortHandCharacterClass()
        {
            var input = @"[\s]";
            var nfa = CreateNfa(input);
            VerifyNfaIsNotNullAndHasValidStartAndEndStates(nfa);

            // start -  whitespace - end

            var start = nfa.Start;
            Assert.AreEqual(1, nfa.Start.Transitions.Count);

            var transition = nfa.Start.Transitions[0];
            var terminalTransition = transition as TerminalNfaTransition;
            Assert.IsNotNull(terminalTransition);

            var whitespaceTerminal = terminalTransition.Terminal as WhitespaceTerminal;
            Assert.IsNotNull(whitespaceTerminal);            
        }

        [TestMethod]
        public void ThompsonConstructionShouldRecognizeNonWhitespaceShortHandCharacterClass()
        {
            var input = @"[\S]";
            var nfa = CreateNfa(input);
            VerifyNfaIsNotNullAndHasValidStartAndEndStates(nfa);

            var start = nfa.Start;
            Assert.AreEqual(1, nfa.Start.Transitions.Count);

            var transition = nfa.Start.Transitions[0];
            var terminalTransition = transition as TerminalNfaTransition;
            Assert.IsNotNull(terminalTransition);

            var negationTerminal = terminalTransition.Terminal as NegationTerminal;
            Assert.IsNotNull(negationTerminal);

            var whitespaceTerminal = negationTerminal.InnerTerminal as WhitespaceTerminal;
            Assert.IsNotNull(whitespaceTerminal);
        }

        [TestMethod]
        public void ThompsonConstructionShouldRecognizeWordShortHandCharacterClass()
        {
            var input = @"[\w]";
            var nfa = CreateNfa(input);
            VerifyNfaIsNotNullAndHasValidStartAndEndStates(nfa);

            var start = nfa.Start;
            Assert.AreEqual(1, nfa.Start.Transitions.Count);

            var transition = nfa.Start.Transitions[0];
            var terminalTransition = transition as TerminalNfaTransition;
            Assert.IsNotNull(terminalTransition);

            var wordTerminal = terminalTransition.Terminal as WordTerminal;
            Assert.IsNotNull(wordTerminal);
        }

        [TestMethod]
        public void ThompsonConstructionShouldRecognizeNonWordShortHandCharacterClass()
        {
            var input = @"[\W]";
            var nfa = CreateNfa(input);
            VerifyNfaIsNotNullAndHasValidStartAndEndStates(nfa);

            var start = nfa.Start;
            Assert.AreEqual(1, nfa.Start.Transitions.Count);

            var transition = nfa.Start.Transitions[0];
            var terminalTransition = transition as TerminalNfaTransition;
            Assert.IsNotNull(terminalTransition);

            var negationTerminal = terminalTransition.Terminal as NegationTerminal;
            Assert.IsNotNull(negationTerminal);

            var wordTerminal = negationTerminal.InnerTerminal as WordTerminal;
            Assert.IsNotNull(wordTerminal);
        }

        [TestMethod]
        public void ThompsonConstructionShouldRecognizeDigitShortHandCharacterClass()
        {
            var input = @"[\d]";
            var nfa = CreateNfa(input);
            VerifyNfaIsNotNullAndHasValidStartAndEndStates(nfa);

            var start = nfa.Start;
            Assert.AreEqual(1, nfa.Start.Transitions.Count);

            var transition = nfa.Start.Transitions[0];
            var terminalTransition = transition as TerminalNfaTransition;
            Assert.IsNotNull(terminalTransition);

            var digitTerminal = terminalTransition.Terminal as DigitTerminal;
            Assert.IsNotNull(digitTerminal);
        }

        [TestMethod]
        public void ThompsonConstructionShouldRecognizeNonDigitShortHandCharacterClass()
        {

            var input = @"[\D]";
            var nfa = CreateNfa(input);
            VerifyNfaIsNotNullAndHasValidStartAndEndStates(nfa);

            var start = nfa.Start;
            Assert.AreEqual(1, nfa.Start.Transitions.Count);

            var transition = nfa.Start.Transitions[0];
            var terminalTransition = transition as TerminalNfaTransition;
            Assert.IsNotNull(terminalTransition);

            var negationTerminal = terminalTransition.Terminal as NegationTerminal;
            Assert.IsNotNull(negationTerminal);

            var digitTerminal = negationTerminal.InnerTerminal as DigitTerminal;
            Assert.IsNotNull(digitTerminal);
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
