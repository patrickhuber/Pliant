using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Earley.Core.Tests
{
    [TestClass]
    public class ProductionTests
    {
        [TestMethod]
        public void Test_Production_That_A_B_ToString_Generates_Appropriate_String()
        {
            var production = new Production("A", new NonTerminal("B"));
            Assert.AreEqual("A -> B", production.ToString());
        }
    }
}
