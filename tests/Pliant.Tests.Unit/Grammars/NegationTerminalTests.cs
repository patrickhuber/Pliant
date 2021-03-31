using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Grammars;

namespace Pliant.Tests.Unit.Grammars
{
    [TestClass]
    public class NegationTerminalTests
    {
        [TestMethod]
        public void NegationTerminalShouldNegateAnyTerminal()
        {
            var anyTerminal = new AnyTerminal();
            var negationTerminal = new NegationTerminal(anyTerminal);
            Assert.IsFalse(negationTerminal.IsMatch('a'));
            Assert.IsFalse(negationTerminal.IsMatch(char.MaxValue));
            Assert.IsFalse(negationTerminal.IsMatch('0'));
        }

        [TestMethod]
        public void NegationTerminalShouldReturnInverseIntervals()
        {
            var anyTerminal = new AnyTerminal();
            var negationTerminal = new NegationTerminal(anyTerminal);
            var intervals = negationTerminal.GetIntervals();
            Assert.AreEqual(0, intervals.Count);            
        }
    }
}
