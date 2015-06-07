using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Grammars;
using Pliant.Charts;

namespace Pliant.Tests.Unit
{
    [TestClass]
    public class StateTests
    {
        [TestMethod]
        public void Test_State_That_ToString_Renders_A_B_C_State()
        {
            var state = new State(
                new Production("A", new NonTerminal("B"), new NonTerminal("C")),
                1,0);
            Assert.AreEqual("A -> B\u25CFC\t\t(0)", state.ToString());
        }
    }
}
