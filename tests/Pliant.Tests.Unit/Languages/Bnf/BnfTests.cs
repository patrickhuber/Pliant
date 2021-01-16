using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Grammars;
using Pliant.Languages.Bnf;
using Pliant.Runtime;
using System.IO;
using System.Text;

namespace Pliant.Tests.Unit.Languages.Bnf
{
    [TestClass]
    public class BnfTests : LanguageBaseTest
    {
        private readonly IGrammar _grammar;

        public BnfTests()
        {
            _grammar = new BnfGrammar();
        }

        [TestInitialize]
        public void InitializeBnfTests()
        {
            Initialize(_grammar);
        }

        [TestMethod]
        public void CanParseSimpleRule()
        {
            var input = "<a> ::= \"a\"";
            ParseAndAcceptInput(input);
        }

        [TestMethod]
        public void CanParseEscapeInDoubleQuoteString()
        {
            var input = "<a> ::= \"\\r\"";
            ParseAndAcceptInput(input);
        }

        [TestMethod]
        public void CanParseEscapesInMultipleDoubleQuoteStrings()
        {
            // 	"\'" | '\"' | "\?" | "\\"
            var input = @"<a> ::= ""\\'"" | '\\""' | ""\\?"" | ""\\\\"" ";
            ParseAndAcceptInput(input);
        }

        [TestMethod]
        public void CanParseConcatenation()
        {
            var input = "<s> ::= \"a\" \"b\"";
            ParseAndAcceptInput(input);
        }

        [TestMethod]
        public void CanParseAlteration()
        {
            var input = "<a> ::= \"a\" | \"b\"";
            ParseAndAcceptInput(input);
        }

        [TestMethod]
        [DeploymentItem("bnf.txt")]
        public void CanParseSelf()
        {
            var input = File.ReadAllText(Path.Combine("Languages","Bnf", "bnf.txt"));
            ParseAndAcceptInput(input);
        }

        [TestMethod]
        [DeploymentItem(@"AnsiC.bnf")]
        public void BnfShouldParseLargeGrammarInFile()
        {            
            var bnf = File.ReadAllText(Path.Combine("Languages", "Bnf", "AnsiC.bnf"));
            Assert.IsFalse(string.IsNullOrEmpty(bnf));
            ParseAndAcceptInput(bnf);
        }

        private string _bnfText = @"
            <syntax>         ::= <rule> | <rule> <syntax>
            <rule>           ::= <identifier> ""::="" <expression> <line-end>
            <expression>     ::= <list> | <list> ""|"" <expression>
            <line-end>       ::= <EOL> | <line-end> <line-end>
            <list>           ::= <term > | <term> <list>
            <term>           ::= <literal > | <identifier>
            <identifier>     ::= ""<"" <rule-name> "">""
            <literal>        ::= '""' <text> '""' | ""'"" <text> ""'""";

        [TestMethod]
        public void BnfShouldProduceParseChartForTextGrammar()
        {
            var grammar = new BnfGrammar();
            var parseEngine = new ParseEngine(grammar);
            var parseRunner = new ParseRunner(parseEngine, _bnfText);

            while (!parseRunner.EndOfStream())
            {
                if (!parseRunner.Read())
                {
                    var position = parseRunner.Position;
                    var startIndex = 0;
                    for (int i = position; i >= 0; i--)
                    {
                        if (_bnfText[i] == '\n' && i > 0)
                            if (_bnfText[i - 1] == '\r')
                            {
                                startIndex = i;
                                break;
                            }
                    }
                    var endIndex = _bnfText.IndexOf(
                        System.Environment.NewLine,
                        position,
                        System.StringComparison.CurrentCulture);
                    endIndex = endIndex < 0 ? _bnfText.Length : endIndex;
                    var length = endIndex - startIndex;
                    var stringBuilder = new StringBuilder();
                    stringBuilder
                        .Append($"Error parsing input string at position {parseRunner.Position}.")
                        .AppendLine()
                        .Append($"start: {startIndex}")
                        .AppendLine()
                        .AppendLine(_bnfText.Substring(startIndex, length));

                    Assert.Fail(stringBuilder.ToString());
                }
            }
            Assert.IsTrue(
                parseRunner.ParseEngine.IsAccepted(),
                $"error at position {parseRunner.Position}");
        }
    }
}
