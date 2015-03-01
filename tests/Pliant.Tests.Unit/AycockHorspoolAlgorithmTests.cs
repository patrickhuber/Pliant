using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;

namespace Pliant.Tests.Unit
{
    [TestClass]
    public class AycockHorspoolAlgorithmTests
    {
        private Grammar _grammar = new GrammarBuilder("S'", p => p
                .Production("S'", r => r.Rule("S"))
                .Production("S",  r => r.Rule("A", "A", "A", "A"))
                .Production("A",  r => r.Rule('a').Rule("E"))
                .Production("E",  r => r.Lambda()))
            .GetGrammar();

        [TestMethod]
        public void Test_AycockHorspoolAlgorithm_That_Vulnerable_Grammar_Accepts_Input()
        {
            var recognizer = new Recognizer(_grammar);
            var chart = recognizer.Parse(new StringReader("a"));
            Assert.IsNotNull(chart);
            Assert.AreEqual(2, chart.Count);

            // a match would has a start state 
            // of origin 0 in the last column 
            var stringIsRecognized = chart[1].Any(state =>
                    state.IsComplete()
                    && state.Origin == 0
                    && state.Production.LeftHandSide.Value.Equals("S'"));
            Assert.IsTrue(stringIsRecognized);
        }
    }
}
