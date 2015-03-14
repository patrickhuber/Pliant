using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Pliant.Tests.Unit
{
    [TestClass]
    public class LexemeTests
    {
        [TestMethod]
        public void Test_Lexeme_That_Consumes_Whitespace()
        {
            var whitespace = new NonTerminal("whitespace");
            var terminal = new WhitespaceTerminal();
            var input = "\t \v\f\r\n";
            var lexeme = new Lexeme(whitespace, terminal, Repetition.ZeroOrMany);
            foreach (var c in input)
                Assert.IsTrue(lexeme.Match(c));
            Assert.AreEqual(input, lexeme.Capture);
        }
    }
}
