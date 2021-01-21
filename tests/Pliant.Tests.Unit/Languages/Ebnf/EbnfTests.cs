using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Grammars;
using Pliant.Languages.Ebnf;
using System.IO;

namespace Pliant.Tests.Unit.Languages.Ebnf
{
    [TestClass]
    public class EbnfTests : LanguageBaseTest
    {
        private IGrammar _ebnfGrammar;

        public EbnfTests()
        {
            _ebnfGrammar = new EbnfGrammar();
        }

        [TestInitialize]
        public void Initialize_Regex_Tests()
        {
            Initialize(_ebnfGrammar);
        }

        [TestMethod]
        public void CanParseSimpleRule()
        {
            var input = "s = \"a\";";
            ParseAndAcceptInput(input);
        }


        [TestMethod]
        public void CanParseConcatenation()
        {
            var input = "twelve = \"1\", \"2\" ;";
            ParseAndAcceptInput(input);
        }

        [TestMethod]
        public void CanParseMultipleRules()
        {
            var input = @"
s = a, b;
b = c;
a = c;";
            ParseAndAcceptInput(input);
        }

        [TestMethod]
        public void CanParseRepetition()
        {
            var input = "a = { b };";
            ParseAndAcceptInput(input);
        }

        [TestMethod]
        public void CanParseAlteration()
        {
            var input = "a = a | b;";
            ParseAndAcceptInput(input);
        }

        [TestMethod]
        public void CanParseOptional()
        {
            var input = "a = [a];";
            ParseAndAcceptInput(input);
        }

        [TestMethod]
        public void CanParseGroup()
        {
            var input = "a = ( a );";
            ParseAndAcceptInput(input);
        }

        [TestMethod]
        [DeploymentItem("ebnf.txt")]
        public void CanParseSelf()
        {
            var input = File.ReadAllText(Path.Combine("Languages", "Ebnf", "ebnf.txt"));
            ParseAndAcceptInput(input);
        }
    }
}
