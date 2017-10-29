using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Builders.Expressions;
using Pliant.Charts;
using Pliant.Grammars;
using Pliant.Runtime;
using Pliant.Tokens;
using Testable;

namespace Pliant.Tests.Unit.Runtime
{
    [TestClass]
    public class AycockHorspoolAlgorithmTests
    {
        [TestMethod]
        public void AycockHorspoolAlgorithmShouldAcceptVulnerableGrammar()
        {
            var a = new TerminalLexerRule(
                new CharacterTerminal('a'),
                new TokenType("a"));

            ProductionExpression 
                SPrime = "S'",
                S = "S",
                A = "A",
                E = "E";

            SPrime.Rule = S;
            S.Rule = (Expr)S | A + A + A + A;
            A.Rule = (Expr)"a" | E;

            var expression = new GrammarExpression(
                SPrime,
                new[] { SPrime, S, A, E });

            var grammar = expression.ToGrammar();

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