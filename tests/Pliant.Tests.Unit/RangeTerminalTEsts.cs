using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Grammars;

namespace Pliant.Tests.Unit
{
    [TestClass]
    public class RangeTerminalTests
    {
        [TestMethod]
        public void Test_RangeTerminal_That_When_Start_Less_Than_Lower_Bound_IsMatch_Returns_False()
        {
            var rangeTerminal = new RangeTerminal('a', 'z');
            Assert.IsFalse(rangeTerminal.IsMatch('0'));
        }

        [TestMethod]
        public void Test_RangeTerminal_That_When_End_Greater_Than_Upper_Bound_IsMatch_Returns_False()
        {
            var rangeTerminal = new RangeTerminal('a', 'z');
            Assert.IsFalse(rangeTerminal.IsMatch('A'));
        }


        [TestMethod]
        public void Test_RangeTerminal_That_When_Character_Between_Bounds_IsMatch_Returns_True()
        {
            var rangeTerminal = new RangeTerminal('a', 'z');
            Assert.IsTrue(rangeTerminal.IsMatch('l'));
        }
    }
}
