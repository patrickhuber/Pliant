using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Pliant.Tests.Unit
{
    [TestClass]
    public class GrammarTests
    {
        [TestMethod]
        public void Test_Grammar_That_RulesFor_Returns_Rules_When_Production_Matches()
        {
            var B = new NonTerminal("B");
            var A = new NonTerminal("A");
            var S = new NonTerminal("S");
            var grammarBuilder = new GrammarBuilder("S", p => p
                .Production("S", r => r
                    .Rule("A")
                    .Rule("B"))
                .Production("A", r => r
                    .Rule('a'))
                .Production("B", r => r
                    .Rule('b')));
            var grammar = grammarBuilder.GetGrammar();
            var rules = grammar.RulesFor(A).ToList();
            Assert.AreEqual(1, rules.Count);
            Assert.AreEqual("A", rules[0].LeftHandSide.Value);
        }
    }
}
