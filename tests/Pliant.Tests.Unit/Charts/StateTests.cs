using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Charts;
using Pliant.Grammars;

namespace Pliant.Tests.Unit
{
    [TestClass]
    public class StateTests
    {
        [TestMethod]
        public void StateToStringShouldCreateCorrectFormat()
        {
            var state = new NormalState(
                new DottedRule(
                    new Production(
                        new NonTerminal("A"), new NonTerminal("B"), new NonTerminal("C")),
                    1), 
                0);
            Assert.AreEqual("A -> B\u25CFC\t\t(0)", state.ToString());
        }
    }
}