﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Automata;

namespace Pliant.Tests.Unit.Automata
{
    [TestClass]
    public class TableFiniteAutomataTests
    {

        [TestMethod]
        public void TableNfaShouldCreateEquivalentTableDfa()
        {
            var tableNfa = new TableNfa(0);
            tableNfa.AddTransition(0, 'a', 1);
            tableNfa.AddTransition(0, 'c', 3);
            tableNfa.AddNullTransition(1, 0);
            tableNfa.AddTransition(1, 'b', 2);
            tableNfa.AddTransition(2, 'a', 1);
            tableNfa.AddTransition(3, 'c', 2);
            tableNfa.AddNullTransition(3, 2);
            tableNfa.SetFinal(2, true);

            var tableDfa = tableNfa.ToDfa();

            Assert.IsNotNull(tableDfa);
            var input = "aaacabac";

            AssertTableDfaCanRecognizeInput(tableDfa, input);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0601:Value type to reference type conversion causing boxing allocation", Justification = "unit test is not critical code")]
        private static void AssertTableDfaCanRecognizeInput(TableDfa tableDfa, string input)
        {
            var state = tableDfa.Start;

            for (int i = 0; i < input.Length; i++)
            {
                var character = input[i];
                var target = tableDfa.Transition(state, character);
                if (target is null)
                    Assert.Fail($"Unable to transition from state {state} with character {character}.");
                state = target.Value;
            }
            Assert.IsTrue(tableDfa.IsFinal(state));
        }
    }
}
