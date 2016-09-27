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

        [TestMethod]
        public void RangeTerminalGetIntervalsShouldReturnSingleIntervalWithMinAndMaxChar()
        {
            var rangeTerminal = new RangeTerminal('a', 'z');
            var intervals = rangeTerminal.GetIntervals();
            Assert.AreEqual(1, intervals.Count);
            Assert.AreEqual('a', intervals[0].Min);
            Assert.AreEqual('z', intervals[0].Max);
        }
    }
}