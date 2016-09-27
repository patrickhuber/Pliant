using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Grammars;

namespace Pliant.Tests.Unit.Grammars
{
    [TestClass]
    public class CharacterTerminalTests
    {
        [TestMethod]
        public void CharacterTerminalShouldMatchSingleCharacter()
        {
            var characterTerminal = new CharacterTerminal('c');
            Assert.IsTrue(characterTerminal.IsMatch('c'));
        }

        [TestMethod]
        public void CharacterTerminalShouldReturnSingleInterval()
        {
            var characterTerminal = new CharacterTerminal('a');
            var intervals = characterTerminal.GetIntervals();
            Assert.AreEqual(1, intervals.Count);
            Assert.AreEqual('a', intervals[0].Min);
            Assert.AreEqual('a', intervals[0].Max);
        }
    }
}
