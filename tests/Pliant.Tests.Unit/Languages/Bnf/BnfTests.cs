using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Grammars;
using Pliant.Languages.Bnf;
using System.IO;

namespace Pliant.Tests.Unit.Languages.Bnf
{
    [TestClass]
    public class BnfTests : LanguageBaseTest
    {
        private IGrammar _grammar;

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
            var input = File.ReadAllText(@"Languages\Bnf\bnf.txt");
            ParseAndAcceptInput(input);
        }
    }
}
