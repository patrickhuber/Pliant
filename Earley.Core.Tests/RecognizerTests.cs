using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;

namespace Earley.Core.Tests
{
    [TestClass]
    public class RecognizerTests
    {
        // S -> S + M | M
        // M -> M * T | T
        // T -> 1 | 2 | 3 | 4
        private readonly Grammar expressionGrammar = new Grammar(
                new Production("S", new NonTerminal("S"), new Terminal("+"), new NonTerminal("M")),
                new Production("S", new NonTerminal("M")),
                new Production("M", new NonTerminal("M"), new Terminal("*"), new NonTerminal("T")),
                new Production("M", new NonTerminal("T")),
                new Production("T", new Terminal("1")),
                new Production("T", new Terminal("2")),
                new Production("T", new Terminal("3")),
                new Production("T", new Terminal("4")));

        // A -> B C
        // B -> b
        // C -> c
        private readonly Grammar abcGrammar = new Grammar(
            new Production(
                "A",
                new NonTerminal("B"),
                new NonTerminal("C")),
            new Production(
                "B",
                new Terminal("b")),
            new Production(
                "C",
                new Terminal("c")));

        [TestMethod]
        public void Test_Recognizer_That_A_B_C_Grammar_Parses_bc()
        {
            
            var recognizer = new Recognizer(abcGrammar);
            var stringBuilder = new StringBuilder("bc");
            var stringBuilderEnumerable = new StringBuilderEnumerable(stringBuilder);
            var chart = recognizer.Parse(stringBuilderEnumerable);
            Assert.IsNotNull(chart);
            Assert.AreEqual(3, chart.Count);
        }

        [TestMethod]
        public void Test_Recognizer_That_S_M_T_Grammar_Parses_2_sum_3_mul_4()
        {
            var recognizer = new Recognizer(expressionGrammar);
            var stringBuilder = new StringBuilder("2+3*4");
            var tokens = new StringBuilderEnumerable(stringBuilder);
            var chart = recognizer.Parse(tokens);
            Assert.IsNotNull(chart);
            Assert.AreEqual(4, chart.Count);
        }
    }
}
