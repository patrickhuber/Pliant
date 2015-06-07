using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Grammars;

namespace Pliant.Tests.Unit
{
    [TestClass]
    public class DigitTerminalTests
    {
        [TestMethod]
        public void Test_DigitTerminal_That_IsMatch_Returns_True_When_Number()
        {
            var digitTerminal = new DigitTerminal();
            Assert.IsTrue (digitTerminal.IsMatch('0'));
        }

        [TestMethod]
        public void Test_DigitTerminal_That_IsMatch_Returns_False_When_Letter()
        {
            var digitTerminal = new DigitTerminal();
            Assert.IsFalse(digitTerminal.IsMatch('a'));
        }
    }
}
