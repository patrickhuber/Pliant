using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;

namespace Pliant.Tests.Unit
{
    [TestClass]
    public class AycockHorspoolAlgorithmTests
    {
        private Grammar _grammar = new GrammarBuilder(g => g
                .Production("S'", p=>p.Rule("S"))
                .Production("S", p=>p.Rule("A", "A", "A", "A"))
                .Production("A", p=>p.Rule('a').Rule("E"))
                .Production("E", p=> p.Lambda()))
            .GetGrammar();

        [TestMethod]
        public void Test_AycockHorspoolAlgorithm_That_Vulnerable_Grammar_Accepts_Input()
        {
            var recognizer = new Recognizer(_grammar);
            var chart = recognizer.Parse(new StringReader("a"));
            Assert.IsNotNull(chart);
            Assert.AreEqual(2, chart.Count);
            var completedStates =
                from state in chart[1]
                where state.IsComplete()
                    && state.Origin == 0
                select state;
            var s1CompletedState = completedStates.FirstOrDefault();
            Assert.IsNotNull(s1CompletedState);
        }
    }
}
