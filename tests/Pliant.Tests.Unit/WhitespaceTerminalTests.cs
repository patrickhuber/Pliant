using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant.Tests.Unit
{
    [TestClass]
    public class WhitespaceTerminalTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test_WhitespaceTerminal_That_Tab_Character_IsMatch_Returns_True()
        {
            var whitespaceTerminal = new WhitespaceTerminal();
            Assert.IsTrue(whitespaceTerminal.IsMatch('\t'));
        }

        [TestMethod]
        public void Test_WhitespaceTerminal_That_NewLine_Character_IsMatch_Returns_True()
        {
            var whitespaceTerminal = new WhitespaceTerminal();
            Assert.IsTrue(whitespaceTerminal.IsMatch('\r'));
        }

        [TestMethod]
        public void Test_WhitespaceTerminal_That_LineFeed_Character_IsMatch_Returns_True()
        {
            var whitespaceTerminal = new WhitespaceTerminal();
            Assert.IsTrue(whitespaceTerminal.IsMatch('\n'));
        }

        [TestMethod]
        public void Test_WhitespaceTerminal_That_Space_Character_IsMatch_Returns_True()
        {
            var whitespaceTerminal = new WhitespaceTerminal();
            Assert.IsTrue(whitespaceTerminal.IsMatch(' '));        
        }
    }
}
