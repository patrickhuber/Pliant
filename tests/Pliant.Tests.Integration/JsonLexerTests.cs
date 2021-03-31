using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Json;
using System.Linq;

namespace Pliant.Tests.Integration
{
    [TestClass]
    public class JsonLexerTests
    {
        [TestMethod]
        public void JsonLexerShouldReadSingleDigit()
        {
            var input = "1";
            var jsonLexer = new JsonLexer();
            var tokens = jsonLexer.Lex(input).ToArray();
            Assert.AreEqual(1, tokens.Length);
            Assert.AreEqual(input, tokens[0].Capture.ToString());
            Assert.AreEqual(0, tokens[0].Position);
        }

        [TestMethod]
        public void JsonLexerShouldReadString()
        {
            var input = "\"this is a string\"";
            var jsonLexer = new JsonLexer();
            var tokens = jsonLexer.Lex(input).ToArray();
            Assert.AreEqual(1, tokens.Length);
            Assert.AreEqual(input, tokens[0].Capture.ToString());
        }

        [TestMethod]
        public void JsonLexerShouldReturnTrue()
        {
            var input = "true";
            var jsonLexer = new JsonLexer();
            var tokens = jsonLexer.Lex(input).ToArray();
            Assert.AreEqual(1, tokens.Length);
            Assert.AreEqual(input, tokens[0].Capture.ToString());
        }

        [TestMethod]
        public void JsonLexerShouldReturnFalse()
        {
            var input = "false";
            var jsonLexer = new JsonLexer();
            var tokens = jsonLexer.Lex(input).ToArray();
            Assert.AreEqual(1, tokens.Length);
            Assert.AreEqual(input, tokens[0].Capture.ToString());
        }

        [TestMethod]
        public void JsonLexerShouldReturnNull()
        {
            var input = "true";
            var jsonLexer = new JsonLexer();
            var tokens = jsonLexer.Lex(input).ToArray();
            Assert.AreEqual(1, tokens.Length);
            Assert.AreEqual(input, tokens[0].Capture.ToString());
        }

        [TestMethod]
        public void JsonLexerShouldReturnArrayTokens()
        {
            var input = "[1,2,3]";
            var jsonLexer = new JsonLexer();
            var tokens = jsonLexer.Lex(input).ToArray();
            Assert.AreEqual(7, tokens.Length);
            for (var i = 0; i < input.Length; i++)
                Assert.AreEqual(input[i], tokens[i].Capture[0]);

            Assert.AreEqual(JsonLexer.OpenBracket, tokens[0].TokenType);
            Assert.AreEqual(JsonLexer.Number, tokens[1].TokenType);
            Assert.AreEqual(JsonLexer.Comma, tokens[2].TokenType);
            Assert.AreEqual(JsonLexer.Number, tokens[3].TokenType);
            Assert.AreEqual(JsonLexer.Comma, tokens[4].TokenType);
            Assert.AreEqual(JsonLexer.Number, tokens[5].TokenType);
            Assert.AreEqual(JsonLexer.CloseBracket, tokens[6].TokenType);
        }

        [TestMethod]
        public void JsonLexerShouldReturnObjectTokens()
        {
            var input = "{\"name\":\"something\",\"id\":12345}";
            var jsonLexer = new JsonLexer();
            var tokens = jsonLexer.Lex(input).ToArray();
            Assert.AreEqual(9, tokens.Length);
            Assert.AreEqual(JsonLexer.OpenBrace, tokens[0].TokenType);
            Assert.AreEqual(JsonLexer.String, tokens[1].TokenType);
            Assert.AreEqual(JsonLexer.Colon, tokens[2].TokenType);
            Assert.AreEqual(JsonLexer.String, tokens[3].TokenType);
            Assert.AreEqual(JsonLexer.Comma, tokens[4].TokenType);
            Assert.AreEqual(JsonLexer.String, tokens[5].TokenType);
            Assert.AreEqual(JsonLexer.Colon, tokens[6].TokenType);
            Assert.AreEqual(JsonLexer.Number, tokens[7].TokenType);
            Assert.AreEqual(JsonLexer.CloseBrace, tokens[8].TokenType);
        }
    }
}
