using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Earley.Core.Tests
{
    [TestClass]
    public class LeoAlgorithmTests
    {
        private IProduction A_Aa = new Production("A", new Terminal("a"), new NonTerminal("A"));
        private IProduction A_ = new Production("A");
        
        [TestMethod]
        public void Test_LeoAlgorithm_That_OptimizeReductionPath_Returns_TopMost_Item_From_Third_Quadrant()
        {
            var grammar = CreateGrammar();
            var chart = CreateChart(grammar);
            var completedState = new State(A_, 0, 2);

            var recognizer = new Recognizer(grammar);
            var invoker = new PrivateObject(recognizer);
            invoker.Invoke("OptimizeReductionPath", completedState, 3, chart);
            
        }

        private Grammar CreateGrammar()
        {
            return new Grammar(
                A_Aa,
                A_);
        }

        private Chart CreateChart(Grammar grammar)
        {            
            var chart = new Chart(grammar);

            // === 0 ===
            // A -> . 'a' A (0)
            // A -> .       (0)
            chart.EnqueueAt(0, new State(A_Aa, 0, 0));
            chart.EnqueueAt(0, new State(A_, 0, 0));

            // === 1 ===
            // A -> 'a' . A (0)
            // A -> . 'a' A (1)
            // A -> 'a' A . (0)
            // A -> .       (1)
            chart.EnqueueAt(1, new State(A_Aa, 1, 0));
            chart.EnqueueAt(1, new State(A_Aa, 0, 1));
            chart.EnqueueAt(1, new State(A_Aa, 2, 0));
            chart.EnqueueAt(1, new State(A_, 0, 1));

            // === 2 === 
            // A -> 'a' . A  (1)
            // A -> . 'a' A  (2)
            // A -> 'a' A .  (1)
            // A -> .        (2)
            chart.EnqueueAt(2, new State(A_Aa, 1, 1));
            chart.EnqueueAt(2, new State(A_Aa, 0, 2));
            chart.EnqueueAt(2, new State(A_Aa, 2, 1));
            chart.EnqueueAt(2, new State(A_, 0, 2));

            // === 3 === 
            // A -> 'a' . A  (2)
            // A -> . 'a' A  (3)
            // A -> 'a' A .  (2)
            // A -> .        (3)
            chart.EnqueueAt(3, new State(A_Aa, 1, 2));
            chart.EnqueueAt(3, new State(A_Aa, 0, 3));
            chart.EnqueueAt(3, new State(A_Aa, 2, 2));
            chart.EnqueueAt(3, new State(A_, 0, 3));
            
            return chart;
        }
    }
}
