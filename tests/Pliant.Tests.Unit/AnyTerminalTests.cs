using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Pliant.Tests.Unit
{
    [TestClass]
    public class AnyTerminalTests
    {
        [TestMethod]
        public void Test_AnyTerminal_That_IsMatch_Returns_True_When_Any_Character_Specified()
        {
            var anyTerminal = new AnyTerminal();
            for (char c = char.MinValue; c < char.MaxValue; c++)
                Assert.IsTrue(anyTerminal.IsMatch(c));
        }
    }
}
