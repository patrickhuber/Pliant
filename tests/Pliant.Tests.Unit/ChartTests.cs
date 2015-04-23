using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Pliant.Tests.Unit
{
    /// <summary>
    /// Summary description for ChartTests
    /// </summary>
    [TestClass]
    public class ChartTests
    {
        public TestContext TestContext { get; set; }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void Test_Chart_That_Enqueue_Avoids_Duplicates()
        {
            var grammar = new GrammarBuilder("L", p => p
                    .Production("L", r => r
                        .Rule("L", new RangeTerminal('a', 'z'))
                        .Rule(new RangeTerminal('a','z'))))
                .GetGrammar();
            var chart = new Chart();
            var firstState = new State(grammar.Productions[0], 0, 1);
            var secondState = new State(grammar.Productions[0], 0, 1);
            chart.Enqueue(0, firstState);
            chart.Enqueue(0, secondState);
            Assert.AreEqual(1, chart.EarleySets[0].Predictions.Count);
        }
    }
}
