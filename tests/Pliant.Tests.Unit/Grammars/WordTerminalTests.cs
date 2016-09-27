using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Grammars;

namespace Pliant.Tests.Unit.Grammars
{
    [TestClass]
    public class WordTerminalTests
    {
        [TestMethod]
        public void WordTerminalShouldMatchWordCharacters()
        {
            var wordTerminal = new WordTerminal();
            Assert.IsTrue(wordTerminal.IsMatch('a'));
            Assert.IsTrue(wordTerminal.IsMatch('0'));
            Assert.IsTrue(wordTerminal.IsMatch('M'));
            Assert.IsTrue(wordTerminal.IsMatch('_'));
        }
    }
}
