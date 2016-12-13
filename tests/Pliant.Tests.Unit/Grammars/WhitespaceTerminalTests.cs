using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Grammars;
using System;

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

        [TestMethod]
        public void WhitespaceTerminalGetIntervalsShouldReturnAllWhitespaceRanges()
        {
            var whitespaceTerminal = new WhitespaceTerminal();
            var intervals = whitespaceTerminal.GetIntervals();
            Assert.AreEqual(9, intervals.Count);
        }
        
    }
}