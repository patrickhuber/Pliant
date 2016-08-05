using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Automata;
using Pliant.Builders.Expressions;
using Pliant.Grammars;
using Pliant.RegularExpressions;
using System.IO;

namespace Pliant.Tests.Unit.Runtime
{
    [TestClass]
    public class LargeFileParseTests
    { 
        public TestContext TestContext { get; set; }

        private static GrammarExpression _grammarExpression;

        private ParseTester _parseTester;

        [ClassInitialize]
        public static void Initialize(TestContext testContext)
        {
            ProductionExpression
                Json = "Json",
                Object = "Object",
                Pair = "Pair",
                PairRepeat = "PairRepeat",
                Array = "Array",
                Value = "Value",
                ValueRepeat = "ValueRepeat";

            var number = Number();
            var @string = String();

            Json.Rule = 
                Value;

            Object.Rule = 
                '{' + PairRepeat + '}';

            PairRepeat.Rule =
                Pair
                | Pair + ',' + PairRepeat
                | (Expr)null;

            Pair.Rule =
                (Expr)@string + ':' + Value;
                        
            Array.Rule =
                '[' + ValueRepeat + ']';

            ValueRepeat.Rule = 
                Value 
                | Value + ',' + ValueRepeat
                | (Expr)null;

            Value.Rule = (Expr)
                @string
                | number
                | Object
                | Array
                | "true"
                | "false"
                | "null";

            _grammarExpression = new GrammarExpression(
                Json, 
                null, 
                new[] { Whitespace() });
        }

        [TestInitialize]
        public void InitializeTest()
        {
            _parseTester = new ParseTester(_grammarExpression);
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
        [DeploymentItem(@"Runtime\10000.json", "Runtime")]
        public void TestCanParseLargeJsonFile()
        {
            var json = File.ReadAllText(Path.Combine(TestContext.TestDeploymentDir, "Runtime", "10000.json"));
            _parseTester.RunParse(json);
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

        private static BaseLexerRule Number()
        {
            // [-+]?[0-9]*\.?[0-9]+
            const string pattern = @"[-+]?[0-9]*[.]?[0-9]+";
            return CreateRegexDfa(pattern);
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
            var regexCompiler = new RegexCompiler(
                new ThompsonConstructionAlgorithm(),
                new SubsetConstructionAlgorithm());
            var dfa = regexCompiler.Compile(regex);
            return new DfaLexerRule(dfa, pattern);
        }

    }
}
