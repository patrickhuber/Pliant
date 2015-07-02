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
        public void Test_Regex_That_Parses_Sequence_Of_Terms_And_Generates_Appropriate_Parse_Tree()
        {
            var input = @"abcdefg";
            var parseInterface = new ParseInterface(_parseEngine, input);
            while (!parseInterface.EndOfStream())
                if (!parseInterface.Read())
                    Assert.Fail("failure at position {0}", parseInterface.Position);
            Assert.IsTrue(_parseEngine.IsAccepted());
            var root = _parseEngine.GetParseForest();
            Assert.IsNotNull(root);

            var regex_0_7 = root as ISymbolNode;
            Assert.IsNotNull(regex_0_7);
            Assert.AreEqual("Regex", (regex_0_7.Symbol as INonTerminal).Value);
            Assert.AreEqual(1, regex_0_7.Children.Count);

            var regex_0_7_1 = regex_0_7.Children[0] as IAndNode;
            Assert.IsNotNull(regex_0_7_1);            
            Assert.AreEqual(1, regex_0_7_1.Children.Count);

            var expression_0_7 = regex_0_7_1.Children[0] as ISymbolNode;
            Assert.IsNotNull(expression_0_7);
            Assert.AreEqual(1, expression_0_7.Children.Count);

            var expression_0_7_1 = expression_0_7.Children[0] as IAndNode;
            Assert.IsNotNull(expression_0_7_1);
            Assert.AreEqual(1, expression_0_7_1.Children.Count);

            var term_0_7 = expression_0_7_1.Children[0] as ISymbolNode;
            Assert.IsNotNull(term_0_7);
            Assert.AreEqual(1, term_0_7.Children.Count);

            var term_0_7_1 = term_0_7.Children[0] as IAndNode;
            Assert.IsNotNull(term_0_7_1);
            Assert.AreEqual(2, term_0_7_1.Children.Count);

            var term_1_7 = term_0_7_1.Children[1] as ISymbolNode;
            Assert.IsNotNull(term_1_7);
            Assert.AreEqual(1, term_1_7.Children.Count);

            var term_1_7_1 = term_1_7.Children[0] as IAndNode;
            Assert.IsNotNull(term_1_7_1);
            Assert.AreEqual(2, term_1_7_1.Children.Count);
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
