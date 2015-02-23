using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Earley.Core.Tests
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
            var grammar = new Grammar(
                new Production(S, B),
                new Production(S, A),
                new Production(B, new Terminal('b')),
                new Production(A, new Terminal('a')));
            var rules = grammar.RulesFor(A).ToList();
            Assert.AreEqual(1, rules.Count);
            Assert.AreEqual("A", rules[0].LeftHandSide.Value);
        }
    }
}
