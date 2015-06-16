using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Grammars;
using Pliant.Lexemes;
using Pliant.Tokens;

namespace Pliant.Tests.Unit
{
    [TestClass]
    public class TerminalLexemeTests
    {
        public TerminalLexemeTests() { }

        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test_TerminalLexeme_When_Character_Matches_IsAcceptted_True()
        {
            var terminalLexeme = new TerminalLexeme(
                new Terminal('c'), 
                new TokenType("c"));
            Assert.IsFalse(terminalLexeme.IsAccepted());
            Assert.IsTrue(terminalLexeme.Scan('c'));
            Assert.IsTrue(terminalLexeme.IsAccepted());
            Assert.IsFalse(terminalLexeme.Scan('c'));
        }

        [TestMethod]
        public void Test_TerminalLexeme_That_Capture_Is_Empty_String()
        {
            var terminalLexeme = new TerminalLexeme(new Terminal('c'), new TokenType("c"));
            Assert.AreEqual(string.Empty, terminalLexeme.Capture);
        }
    }
}
