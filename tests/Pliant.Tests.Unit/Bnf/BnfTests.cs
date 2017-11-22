using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Bnf;
using Pliant.Runtime;
using System.IO;
using System.Text;

namespace Pliant.Tests.Unit.Bnf
{
    /// <summary>
    ///     <see cref="http://cui.unige.ch/db-research/Enseignement/analyseinfo/AboutBNF.html"/>
    /// </summary>
    [TestClass]
    public class BnfTests
    {
        public TestContext TestContext { get; set; }

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

        [TestMethod]
        [DeploymentItem(@"Bnf\AnsiC.bnf", "Bnf")]
        public void BnfShouldParseLargeGrammarInFile()
        {
            var bnf = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "Bnf", "AnsiC.bnf"));
            Assert.IsFalse(string.IsNullOrEmpty(bnf));

            var grammar = new BnfGrammar();
            var parseEngine = new ParseEngine(grammar);
            var parseRunner = new ParseRunner(parseEngine, bnf);

            while (!parseRunner.EndOfStream())
            {
                if (!parseRunner.Read())
                    Assert.Fail($"Error Parsing At Position {parseRunner.Position}");
            }
            Assert.IsTrue(parseEngine.IsAccepted());
        }
    }
}