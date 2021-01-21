using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Captures;
using Pliant.Grammars;
using Pliant.Tokens;

namespace Pliant.Tests.Unit
{
    [TestClass]
    public class TerminalLexemeTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void TerminalLexemShouldWhileAcceptedContinuesToMatch()
        {
            var input = "c";
            var segment = input.AsCapture();
            var terminalLexeme = new TerminalLexeme(
                new CharacterTerminal('c'),
                new TokenType("c"),
                segment, 
                0);
            Assert.IsFalse(terminalLexeme.IsAccepted());
            Assert.IsTrue(terminalLexeme.Scan());
            Assert.IsTrue(terminalLexeme.IsAccepted());
            Assert.IsFalse(terminalLexeme.Scan());
        }

        [TestMethod]
        public void TerminalLexemeResetShouldClearPreExistingValues()
        {
            var input = "c";
            var segment = input.AsCapture();
            var terminalLexeme = new TerminalLexeme(
                new CharacterTerminal('c'),
                new TokenType("c"),
                segment,
                0);

            Assert.IsTrue(terminalLexeme.Scan());
            terminalLexeme.Reset(
                new TerminalLexerRule(new CharacterTerminal('a'), new TokenType("a")),                
                0);

            Assert.AreEqual(0, terminalLexeme.Position);            
        }
    }
}