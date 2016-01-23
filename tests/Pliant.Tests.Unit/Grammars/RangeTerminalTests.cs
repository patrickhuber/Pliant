using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Grammars;

namespace Pliant.Tests.Unit
{
    [TestClass]
    public class RangeTerminalTests
    {
        [TestMethod]
        public void RangeTerminalWhenInputIsLessThanLowerBoundShouldFail()
        {
            var rangeTerminal = new RangeTerminal('a', 'z');
            Assert.IsFalse(rangeTerminal.IsMatch('0'));
        }

        [TestMethod]
        public void RangeTerminalWhenInputGreaterThanUpperBoundShouldFail()
        {
            var rangeTerminal = new RangeTerminal('a', 'z');
            Assert.IsFalse(rangeTerminal.IsMatch('A'));
        }

        [TestMethod]
        public void RangeTerminalWhenInputBetweenBoundsShouldMatch()
        {
            var rangeTerminal = new RangeTerminal('a', 'z');
            Assert.IsTrue(rangeTerminal.IsMatch('l'));
        }
    }
}