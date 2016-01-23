using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Builders;
using System.Linq;

namespace Pliant.Tests.Unit
{
    [TestClass]
    public class GrammarTests
    {
        [TestMethod]
        public void GrammarRulesForWhenProductionMatchesShouldReturnRules()
        {
            ProductionBuilder B = "B", A = "A", S = "S";
            S.Definition = A | B;
            A.Definition = 'a';
            B.Definition = 'b';
            var grammar = new GrammarBuilder(S, new[] { S, A, B })
                .ToGrammar();
            var rules = grammar.RulesFor(A.LeftHandSide).ToList();
            Assert.AreEqual(1, rules.Count);
            Assert.AreEqual("A", rules[0].LeftHandSide.Value);
        }
    }
}