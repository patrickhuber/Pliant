using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Grammars;

namespace Pliant.Tests.Unit
{
    [TestClass]
    public class WhitespaceTerminalTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void WhitespaceTerminalShouldMatchTabCharacter()
        {
            var whitespaceTerminal = new WhitespaceTerminal();
            Assert.IsTrue(whitespaceTerminal.IsMatch('\t'));
        }

        [TestMethod]
        public void WhitespaceTerminalShouldMatchNewLineCharacter()
        {
            var whitespaceTerminal = new WhitespaceTerminal();
            Assert.IsTrue(whitespaceTerminal.IsMatch('\r'));
        }

        [TestMethod]
        public void WhitespaceTerminalShouldMatchLineFeed()
        {
            var whitespaceTerminal = new WhitespaceTerminal();
            Assert.IsTrue(whitespaceTerminal.IsMatch('\n'));
        }

        [TestMethod]
        public void WhitespaceShouldMatchSpaceCharacter()
        {
            var whitespaceTerminal = new WhitespaceTerminal();
            Assert.IsTrue(whitespaceTerminal.IsMatch(' '));
        }
    }
}