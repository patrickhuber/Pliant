using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Grammars;

namespace Pliant.Tests.Unit
{
    [TestClass]
    public class DigitTerminalTests
    {
        [TestMethod]
        public void DigitTerminalGivenNumberShouldMatch()
        {
            var digitTerminal = new DigitTerminal();
            Assert.IsTrue(digitTerminal.IsMatch('0'));
        }

        [TestMethod]
        public void DigitTerminalGivenLetterShouldFailMatch()
        {
            var digitTerminal = new DigitTerminal();
            Assert.IsFalse(digitTerminal.IsMatch('a'));
        }

        [TestMethod]
        public void DigitTerminalGetIntervalsShouldReturnSingleIntervalWithZeroToNineRange()
        {
            var digitTerminal = new DigitTerminal();
            var intervals = digitTerminal.GetIntervals();
            Assert.AreEqual(1, intervals.Count);
            Assert.AreEqual('0', intervals[0].Min);
            Assert.AreEqual('9', intervals[0].Max);
        }
    }
}