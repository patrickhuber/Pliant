using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace Pliant.Tests.Unit
{
    /// <summary>
    /// Summary description for PulseRecognizerTests
    /// </summary>
    [TestClass]
    public class PulseRecognizerTests
    {
        public TestContext TestContext { get; set; }
        
        [TestMethod]
        public void Test_PulseRecognizer_That_Right_Recursive_Quasi_Complete_Items_Are_Leo_Optimized()
        {
            var grammar = new GrammarBuilder("S", p => p
                    .Production("S", r => r
                        .Rule("S", "L")
                        .Lambda())
                    .Production("L", r => r
                        .Rule(new RangeTerminal('a', 'z'), "L`"))
                    .Production("L`", r => r
                        .Rule(new RangeTerminal('a', 'z'), "L`")
                        .Lambda()))
                .GetGrammar();
            var input = "thisisonelonginputstring";
            var recognizer = new PulseRecognizer(grammar);
            Recognize(recognizer, input);

            // when this count is < 10 we know that quasi complete items are being processed successfully
            Assert.IsTrue(recognizer.Chart.EarleySets[23].Completions.Count < 10);
        }

        [TestMethod]
        public void Test_PulseRecognizer_That_Right_Recursion_Is_Not_O_N_3()
        {
            var grammar = new GrammarBuilder("A", p => p
                .Production("A", r => r
                    .Rule('a', "A")
                    .Lambda()))
            .GetGrammar();

            const string input = "aaaaa";
            var recognizer = new PulseRecognizer(grammar);
            Recognize(recognizer, input);
            
            var chart = recognizer.Chart;
            // -- 0 --
            // A ->.a A		    (0)	 # Start
            // A ->.			(0)	 # Start
            //
            // ...
            // -- n --
            // n	A -> a.A		(n-1)	 # Scan a
            // n	A ->.a A		(n)	 # Predict
            // n	A ->.			(n)	 # Predict
            // n	A -> a A.		(n)	 # Predict
            // n	A : A -> a A.	(0)	 # Transition
            // n	A -> a A.		(0)	 # Complete
            Assert.AreEqual(input.Length + 1, chart.Count);
            var lastEarleySet = chart.EarleySets[chart.EarleySets.Count - 1];
            Assert.AreEqual(3, lastEarleySet.Completions.Count);
            Assert.AreEqual(1, lastEarleySet.Transitions.Count);
            Assert.AreEqual(1, lastEarleySet.Predictions.Count);
            Assert.AreEqual(1, lastEarleySet.Scans.Count);
        }

        [TestMethod]
        public void Test_PulseRecognizer_That_Hidden_Right_Recursion_Is_Optimized()
        {
            // S -> a S E | null
            // E -> null
            Assert.Inconclusive();
        }

        [TestMethod]
        public void Test_PulseRecognizer_That_Intermediate_Step_Creates_Transition_Items()
        {
            var grammar = new GrammarBuilder("S", p => p
                .Production("S", r => r
                    .Rule("A"))
                .Production("A", r => r
                    .Rule('a', "B"))
                .Production("B", r => r
                    .Rule("A")
                    .Rule('b')))
            .GetGrammar();
            const string input = "aaab";
            Recognize(grammar, input);
        }

        private static void Recognize(Grammar grammar, string input)
        {
            var recognizer = new PulseRecognizer(grammar);
            Recognize(recognizer, input);
        }

        private static void Recognize(PulseRecognizer recognizer, string input)
        {
            foreach (var c in input)
                Assert.IsTrue(recognizer.Pulse(c));

            Assert.IsTrue(recognizer.IsAccepted());
        }
    }
}
