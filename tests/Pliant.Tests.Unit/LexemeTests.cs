using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Pliant.Tests.Unit
{
    [TestClass]
    public class LexemeTests
    {
        [TestMethod]
        public void Test_Lexeme_That_Consumes_Whitespace()
        {
            var whitespace = new NonTerminal("whitespace");
            var terminal = new WhitespaceTerminal();
            var input = "\t \v\f\r\n";
            var lexerRule = new LexerRule(whitespace, new[] { terminal }, Repetition.ZeroOrMany);
            var lexeme = new Lexeme(lexerRule);
            foreach (var c in input)
                Assert.IsTrue(lexeme.Match(c));
            Assert.AreEqual(input, lexeme.Capture);
        }

        [TestMethod]
        public void Test_Lexeme_That_Consumes_Character_Sequence()
        {
            var letter = new NonTerminal("letter");
            var terminal = new RangeTerminal('a', 'z');
            var input = "thisisabunchofletters";
            var lexerRule = new LexerRule(letter, new[]{ terminal }, Repetition.OneOrMany);
            var lexeme = new Lexeme(lexerRule);
            foreach (var c in input)
                Assert.IsTrue(lexeme.Match(c));
            Assert.AreEqual(input, lexeme.Capture);
        }
    }
}
