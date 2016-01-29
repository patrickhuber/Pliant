using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Grammars;

namespace Pliant.Tests.Unit
{
    [TestClass]
    public class ProductionTests
    {
        [TestMethod]
        public void ProductionToStringShouldGenerateCorrectlyFormattedString()
        {
            var production = new Production(new NonTerminal("A"), new NonTerminal("B"));
            Assert.AreEqual("A -> B", production.ToString());
        }
    }
}