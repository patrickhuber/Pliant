using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Builders;
using Pliant.Charts;
using Pliant.Grammars;
using Pliant.Tokens;

namespace Pliant.Tests.Unit
{
    [TestClass]
    public class AycockHorspoolAlgorithmTests
    {
        [TestMethod]
        public void Test_AycockHorspoolAlgorithm_That_Vulnerable_Grammar_Accepts_Input()
        {
            var a = new TerminalLexerRule(
                new Terminal('a'),
                new TokenType("a"));

            var grammar = new GrammarBuilder("S'", p => p
                .Production("S'", r => r
                    .Rule("S"))
                .Production("S", r => r
                   .Rule("A", "A", "A", "A"))
                .Production("A", r => r
                   .Rule(a)
                   .Rule("E"))
                .Production("E", r => r
                   .Lambda()))
            .GetGrammar();

            var parseEngine = new ParseEngine(grammar);
            parseEngine.Pulse(new Token("a", 0, a.TokenType));

            var privateObject = new PrivateObject(parseEngine);
            var chart = privateObject.GetField("_chart") as Chart;

            Assert.IsNotNull(chart);
            Assert.AreEqual(2, chart.Count);
            Assert.IsTrue(parseEngine.IsAccepted());
        }
    }
}
