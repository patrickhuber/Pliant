using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Builders;
using Pliant.Charts;
using Pliant.Grammars;

namespace Pliant.Tests.Unit
{
    /// <summary>
    /// Summary description for ChartTests
    /// </summary>
    [TestClass]
    public class ChartTests
    {
        [TestMethod]
        public void ChartEnqueShouldAvoidDuplication()
        {
            ProductionBuilder L = "L";
            var aToZ = new RangeTerminal('a', 'z');
            L.Definition = L + aToZ | aToZ;
            var grammar = new GrammarBuilder(L, new[] { L }).ToGrammar();
            var chart = new Chart();
            var firstState = new State(grammar.Productions[0], 0, 1);
            var secondState = new State(grammar.Productions[0], 0, 1);
            chart.Enqueue(0, firstState);
            chart.Enqueue(0, secondState);
            Assert.AreEqual(1, chart.EarleySets[0].Predictions.Count);
        }
    }
}