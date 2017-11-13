using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Automata;
using Pliant.Grammars;
using Pliant.Json;
using Pliant.RegularExpressions;
using Pliant.Runtime;
using Pliant.Tests.Common;
using System.IO;

namespace Pliant.Tests.Integration.Runtime
{
    [TestClass]
    public class LargeFileParseTests
    {
        public TestContext TestContext { get; set; }

        private static IGrammar _grammar;

        private ParseTester _parseTester;
        private ParseTester _compressedParseTester;
        private ParseTester _marpaParseTester;
        
        [ClassInitialize]
#pragma warning disable CC0057 // Unused parameters
#pragma warning disable RECS0154 // Parameter is never used
        public static void Initialize(TestContext testContext)
#pragma warning restore RECS0154 // Parameter is never used
#pragma warning restore CC0057 // Unused parameters
        {
            _grammar = new JsonGrammar();
        }

        [TestInitialize]
        public void InitializeTest()
        {
            _parseTester = new ParseTester(_grammar);
            var preComputedGrammar = new PreComputedGrammar(_grammar);
            _compressedParseTester = new ParseTester(
                new DeterministicParseEngine(preComputedGrammar));
            _marpaParseTester = new ParseTester(
                new MarpaParseEngine(preComputedGrammar));
        }

        [TestMethod]
        public void TestCanParseJsonArray()
        {
            var json = @"[""one"", ""two""]";
            _parseTester.RunParse(json);
        }

        [TestMethod]
        public void TestCanParseJsonObject()
        {
            var json = @"
            {
                ""firstName"":""Patrick"", 
                ""lastName"": ""Huber"",
                ""id"": 12345
            }";
            _parseTester.RunParse(json);
        }

        [TestMethod]
        [DeploymentItem(@"10000.json")]
        public void TestCanParseLargeJsonFile()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "10000.json");
            using (var stream = File.OpenRead(path))
            using (var reader = new StreamReader(stream))
            {
                _parseTester.RunParse(reader);
            }
        }

        [TestMethod]
        [DeploymentItem(@"10000.json")]
        public void TestCanParseLargeJsonFileWithCompression()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "10000.json");
            using (var stream = File.OpenRead(path))
            using (var reader = new StreamReader(stream))
            {
                _compressedParseTester.RunParse(reader);
            }
        }

        [TestMethod]
        [DeploymentItem(@"10000.json")]
        public void TestCanParseLargeJsonFileWithMarpa()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "10000.json");
            using (var stream = File.OpenRead(path))
            using (var reader = new StreamReader(stream))
            {
                _marpaParseTester.RunParse(reader);
            }
        }

        [TestMethod]
        public void TestCanParseJsonArrayWithCompression()
        {
            var json = @"[""one"", ""two""]";
            _compressedParseTester.RunParse(json);
        }

        [TestMethod]
        [DeploymentItem(@"10000.json")]
        public void TestCanParseLargeJsonFileWithCustomLexer()
        {
            var parser = _parseTester.ParseEngine;
            RunParseWithCustomLexer(parser);
        }

        [TestMethod]
        public void TestCanParseLargeJsonFileWithCustomLexerAndMarpa()
        {
            var parser = _marpaParseTester.ParseEngine;
            RunParseWithCustomLexer(parser);
        }

        [TestMethod]
        public void TestCanParseLargeJsonFileWithCustomLexerAndCompression()
        {
            var parser = _compressedParseTester.ParseEngine;
            RunParseWithCustomLexer(parser);
        }

        private static void RunParseWithCustomLexer(IParseEngine parser)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "10000.json");
            var jsonLexer = new JsonLexer();
            using (var stream = File.OpenRead(path))
            using (var reader = new StreamReader(stream))
            {
                var tokens = jsonLexer.Lex(reader);
                foreach (var token in tokens)
                    if (token.TokenType != JsonLexer.Whitespace)
                        if (!parser.Pulse(token))
                            Assert.Fail($"unable to parse token {token.TokenType} at {token.Position}");
            }
            if (!parser.IsAccepted())
                Assert.Fail("Parse was not accepted");
        }

        [TestMethod]
        public void TestCanParseJsonObjectWithCompression()
        {
            var json = @"
            {
                ""firstName"":""Patrick"", 
                ""lastName"": ""Huber"",
                ""id"": 12345
            }";
            _compressedParseTester.RunParse(json);
        }

        private static ILexerRule Whitespace()
        {
            var start = new DfaState();
            var end = new DfaState(isFinal: true);
            var transition = new DfaTransition(
                new WhitespaceTerminal(),
                end);
            start.AddTransition(transition);
            end.AddTransition(transition);
            return new DfaLexerRule(start, "\\w+");
        }

        private static BaseLexerRule String()
        {
            // ["][^"]+["]
            const string pattern = "[\"][^\"]+[\"]";
            return CreateRegexDfa(pattern);
        }

        private static BaseLexerRule CreateRegexDfa(string pattern)
        {
            var regexParser = new RegexParser();
            var regex = regexParser.Parse(pattern);
            var regexCompiler = new RegexCompiler();
            var dfa = regexCompiler.Compile(regex);
            return new DfaLexerRule(dfa, pattern);
        }

    }
}
