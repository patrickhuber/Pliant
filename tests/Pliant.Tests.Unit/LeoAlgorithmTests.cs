using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Pliant.Tests.Unit
{
    [TestClass]
    public class LeoAlgorithmTests
    {
        private IProduction A_aA = new Production("A", new Terminal('a'), new NonTerminal("A"));
        private IProduction A_ = new Production("A");
        
        [TestMethod]
        public void Test_LeoAlgorithm_That_OptimizeReductionPath_Returns_TopMost_Item_From_Third_Quadrant()
        {
            var grammar = CreateGrammar();
            var chart = CreateChart(grammar);
            var completedState = new State(A_, 0, 2);

            var recognizer = new PulseRecognizer(grammar);
            var invoker = new PrivateObject(recognizer);
            invoker.Invoke("OptimizeReductionPath", completedState.Production.LeftHandSide, 3, chart);
            Assert.IsTrue(chart.EarleySets[3].Transitions.Any());
        }

        
        private Grammar CreateGrammar()
        {
            var grammarBuilder = new GrammarBuilder(
                "A", p => p
                .Production("A", r=>r
                    .Rule('a', "A")
                    .Lambda()));
            return grammarBuilder.GetGrammar();
        }

        private Chart CreateChart(Grammar grammar)
        {            
            var chart = new Chart();

            // === 0 ===
            // A -> . 'a' A (0)
            // A -> .       (0)
            chart.Enqueue(0, new State(A_aA, 0, 0));
            chart.Enqueue(0, new State(A_, 0, 0));

            // === 1 ===
            // A -> 'a' . A (0)
            // A -> . 'a' A (1)
            // A -> 'a' A . (0)
            // A -> .       (1)
            chart.Enqueue(1, new State(A_aA, 1, 0));
            chart.Enqueue(1, new State(A_aA, 0, 1));
            chart.Enqueue(1, new State(A_aA, 2, 0));
            chart.Enqueue(1, new State(A_, 0, 1));

            // === 2 === 
            // A -> 'a' . A  (1)
            // A -> . 'a' A  (2)
            // A -> 'a' A .  (1)
            // A -> .        (2)
            chart.Enqueue(2, new State(A_aA, 1, 1));
            chart.Enqueue(2, new State(A_aA, 0, 2));
            chart.Enqueue(2, new State(A_aA, 2, 1));
            chart.Enqueue(2, new State(A_, 0, 2));

            // === 3 === 
            // A -> 'a' . A  (2)
            // A -> . 'a' A  (3)
            // A -> 'a' A .  (2)
            // A -> .        (3)
            chart.Enqueue(3, new State(A_aA, 1, 2));
            chart.Enqueue(3, new State(A_aA, 0, 3));
            chart.Enqueue(3, new State(A_aA, 2, 2));
            chart.Enqueue(3, new State(A_, 0, 3));
            
            return chart;
        }
    }
}
