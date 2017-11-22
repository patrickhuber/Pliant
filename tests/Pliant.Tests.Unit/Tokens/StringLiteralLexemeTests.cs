using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Tokens;
using Pliant.Grammars;

namespace Pliant.Tests.Unit.Tokens
{
    [TestClass]
    public class StringLiteralLexemeTests
    {
        [TestMethod]
        public void StringLiteralLexemeResetShouldResetLexemeValues()
        {
            var abc123LexerRule = new StringLiteralLexerRule("abc123");
            var zyx654LexerRule = new StringLiteralLexerRule("zyx654");
            
            var lexeme = new StringLiteralLexeme(abc123LexerRule, 0);
            const string input = "abc123";
            for (var i = 0; i < input.Length; i++)
            {
                var result = lexeme.Scan(input[i]);
                if (!result)
                    Assert.Fail($"Did not recognize character {input[i]}");
            }

            lexeme.Reset(zyx654LexerRule, 50);
            Assert.AreEqual(string.Empty, lexeme.Value);
            Assert.AreEqual(50, lexeme.Position);
            Assert.AreEqual(zyx654LexerRule.LexerRuleType, lexeme.LexerRule.LexerRuleType);
            Assert.AreEqual(zyx654LexerRule.TokenType, lexeme.TokenType);
        }
    }
}
