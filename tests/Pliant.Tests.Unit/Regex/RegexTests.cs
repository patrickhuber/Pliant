using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Grammars;
using Pliant.Nodes;
using Pliant.Regex;

namespace Pliant.Tests.Unit
{
    /// <summary>
    /// Summary description for RegexTests
    /// </summary>
    [TestClass]
    public class RegexTests
    {
        IParseEngine _parseEngine;
        IGrammar _regexGrammar;

        [TestInitialize]
        public void Initialize_Regex_Tests()
        {
            _parseEngine = new ParseEngine(_regexGrammar);
        }

        public RegexTests()
        {
            _regexGrammar = new RegexGrammar();            
        }

        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test_Regex_That_Parses_Single_Character()
        {
            var input = "a";
            ParseAndAcceptInput(input);
        }

        [TestMethod]
        public void Test_Regex_That_Parses_String_Literal()
        {
            var input = "abc";
            ParseAndAcceptInput(input);
        }

        [TestMethod]
        public void Test_Regex_That_Parses_Optional_Character_Class()
        {
            var input = "a?";
            ParseAndAcceptInput(input);
        }

        [TestMethod]
        public void Test_Regex_That_Parses_Whitespace_Character_Class()
        {
            var input = @"[\s]+";
            ParseAndAcceptInput(input);
        }

        [TestMethod]
        public void Test_Regex_That_Parses_Range_Character_Class()
        {
            var input = @"[a-z]";
            ParseAndAcceptInput(input);
        }

        [TestMethod]
        public void Test_Regex_That_Parses_Any_Character_Class()
        {
            var input = @".*";
            ParseAndAcceptInput(input);
        }

        [TestMethod]
        public void Test_Regex_That_Fails_On_MisMatched_Parenthesis()
        {
            var input = "(a";
            ParseAndNotAcceptInput(input);
        }
        [TestMethod]
        public void Test_Regex_That_Fails_On_MisMatched_Brackets()
        {
            var input = "[abc";
            ParseAndNotAcceptInput(input);
        }

        [TestMethod]
        public void Test_Regex_That_Parses_Empty_Group()
        {
            var input = "()";
            ParseAndAcceptInput(input);
        }

        [TestMethod]
        public void Test_Regex_That_Parses_Empty_String()
        {
            var input = "";
            ParseAndAcceptInput(input);
        }

        [TestMethod]
        public void Test_Regex_That_Parses_Negative_Character_Class()
        {
            var input = "[^abc]";
            ParseAndAcceptInput(input);
        }

        [TestMethod]
        public void Test_Regex_That_Parses_Open_Bracket_In_Character_Class()
        {
            var input = "[\\]]";
            ParseAndAcceptInput(input);
        }

        private void ParseAndAcceptInput(string input)
        {
            ParseInput(input);
            Accept();
        }

        private void ParseAndNotAcceptInput(string input)
        {
            ParseInput(input);
            NotAccept();
        }

        private void ParseInput(string input)
        {
            var parseInterface = new ParseInterface(_parseEngine, input);
            for (int i = 0; i < input.Length; i++)
                Assert.IsTrue(parseInterface.Read(),
                    string.Format("Line 0, Column {1} : Invalid Character '{0}'",
                        input[i],
                        _parseEngine.Location));
        }

        private void Accept()
        {
            Assert.IsTrue(_parseEngine.IsAccepted(), "input was not recognized");
        }

        private void NotAccept()
        {
            Assert.IsFalse(_parseEngine.IsAccepted(), "input was recognized");
        }
    }
}
