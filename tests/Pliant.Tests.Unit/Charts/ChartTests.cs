using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Builders.Expressions;
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
            ProductionExpression L = "L";
            var aToZ = new RangeTerminal('a', 'z');
            L.Rule = L + aToZ | aToZ;
            var grammar = new GrammarExpression(L, new[] { L }).ToGrammar();
            var chart = new Chart();
            var dottedRule = new DottedRule(grammar.Productions[0], 0);
            var firstState = new NormalState(dottedRule, 1);
            var secondState = new NormalState(dottedRule, 1);
            chart.Enqueue(0, firstState);
            chart.Enqueue(0, secondState);
            Assert.AreEqual(1, chart.EarleySets[0].Predictions.Count);
        }
    }
}