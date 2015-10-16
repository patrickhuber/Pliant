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
                new CharacterTerminal('a'),
                new TokenType("a"));

            ProductionBuilder SPrime = "S'";
            ProductionBuilder S = "S";
            ProductionBuilder A = "A";
            ProductionBuilder E = "E";

            SPrime.Definition = S;
            S.Definition = (_) S | A + A + A + A;
            A.Definition = (_)"a" | E;

            var grammarBuilder = new GrammarBuilder(
                SPrime, 
                new[] { SPrime, S, A, E });

            var grammar = grammarBuilder.ToGrammar();
            
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
