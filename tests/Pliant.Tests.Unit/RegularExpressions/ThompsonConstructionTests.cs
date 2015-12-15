using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Automata;
using Pliant.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

namespace Pliant.Tests.Unit.RegularExpressions
{
    [TestClass]
    public class ThompsonConstructionTests
    {
        [TestMethod]
        public void Test_ThompsonConstruction_Given_Null_Expression_Creates_Null_Nfa()
        {
            var input = "";
            var nfa = CreateNfa(input);
            Assert.IsNotNull(nfa);
            Assert.IsFalse(nfa.End.Transitions.Any());
            var list = new List<INfaTransition>(nfa.Start.Transitions);
            Assert.AreEqual(1, list.Count);
            var nullNfaTransition = nfa.Start.Transitions.First() as NullNfaTransition;
            Assert.AreEqual(nfa.End, nullNfaTransition.Target);
        }
        
        [TestMethod]
        public void Test_ThompsonConstruction_Given_Character_Expression_Creates_Character_Nfa()
        {
            var input = "a";
            var nfa = CreateNfa(input);
            Assert.IsNotNull(nfa);
        }

        private static INfa CreateNfa(string input)
        {
            var regex = new RegexParser().Parse(input);
            return new ThompsonConstructionAlgorithm().Transform(regex);
        }
    }
}
