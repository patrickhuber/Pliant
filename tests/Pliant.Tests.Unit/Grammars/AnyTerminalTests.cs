using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Grammars;

namespace Pliant.Tests.Unit
{
    [TestClass]
    public class AnyTerminalTests
    {
        [TestMethod]
        public void AnyTerminalIsMatchShouldReturnTrueWhenAnyCharacterSpecified()
        {
            var anyTerminal = new AnyTerminal();
            for (char c = char.MinValue; c < char.MaxValue; c++)
                Assert.IsTrue(anyTerminal.IsMatch(c));
        }
    }
}