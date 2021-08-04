﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Tokens;
using Pliant.Grammars;
using Pliant.Captures;

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

            const string input = "abc123";
            var segment = input.AsCapture();
            var lexeme = new StringLiteralLexeme(abc123LexerRule, segment, 0);
            for (var i = 0; i < input.Length; i++)
            {
                var result = lexeme.Scan();
                if (!result)
                    Assert.Fail($"Did not recognize character '{input[i]}' at position {i}");
            }

            lexeme.Reset(zyx654LexerRule, 0);
            Assert.AreEqual(string.Empty, lexeme.Capture.ToString());
            Assert.AreEqual(0, lexeme.Position);
            Assert.AreEqual(zyx654LexerRule.LexerRuleType, lexeme.LexerRule.LexerRuleType);
            Assert.AreEqual(zyx654LexerRule.TokenType, lexeme.TokenType);
        }
    }
}
