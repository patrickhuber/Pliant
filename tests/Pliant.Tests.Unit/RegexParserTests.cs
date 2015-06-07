using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Pliant.Tests.Unit
{
    [TestClass]
    public class RegexParserTests
    {
        [TestMethod]
        public void Test_RegexParser_That_Whitespace_OneOrMany_Returns_Whitespace_Grammar()
        {
            var input = "\\s+";
            var regexParser = new RegexParser();
            var grammar = regexParser.Parse(input);

            Assert.IsNotNull(grammar);
            Assert.AreEqual(0, grammar.Ignores.Count);
            Assert.AreEqual("whitespace", grammar.Start.Value);
            Assert.AreEqual(2, grammar.Productions.Count);
        }
    }
}
