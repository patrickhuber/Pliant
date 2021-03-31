using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Grammars;
using Pliant.Languages.Regex;

namespace Pliant.Tests.Unit.Languages.Regex
{
    /// <summary>
    /// Summary description for RegexTests
    /// </summary>
    [TestClass]
    public class RegexTests : LanguageBaseTest
    {        
        private IGrammar _regexGrammar;

        [TestInitialize]
        public void Initialize_Regex_Tests()
        {
            Initialize(_regexGrammar);
        }

        public RegexTests()
        {
            _regexGrammar = new RegexGrammar();
        }

        [TestMethod]
        public void RegexShouldParseSingleCharacter()
        {
            var input = "a";
            ParseAndAcceptInput(input);
        }

        [TestMethod]
        public void RegexShouldParseStringLiteral()
        {
            var input = "abc";
            ParseAndAcceptInput(input);
        }

        [TestMethod]
        public void RegexShouldParseOptionalCharacter()
        {
            var input = "a?";
            ParseAndAcceptInput(input);
        }

        [TestMethod]
        public void RegexShouldParseWhitespaceCharacterClass()
        {
            var input = @"[\s]+";
            ParseAndAcceptInput(input);
        }

        [TestMethod]
        public void RegexShouldParseRangeCharacterClass()
        {
            var input = @"[a-z]";
            ParseAndAcceptInput(input);
        }

        [TestMethod]
        public void RegexShouldParseAnyCharacterClass()
        {
            var input = @".*";
            ParseAndAcceptInput(input);
        }

        [TestMethod]
        public void RegexFailOnMisMatchedParenthesis()
        {
            var input = "(a";
            ParseAndNotAcceptInput(input);
        }

        [TestMethod]
        public void RegexShouldFailOnMisMatchedBrackets()
        {
            var input = "[abc";
            ParseAndNotAcceptInput(input);
        }

        [TestMethod]
        public void RegexShouldFailEmptyGroup()
        {
            var input = "()";
            FailParseAtPosition(input, 1);
        }

        [TestMethod]
        public void RegexShouldFailEmptyInput()
        {
            var input = "";
            ParseAndNotAcceptInput(input);
        }

        [TestMethod]
        public void RegexShouldParseNegativeCharacterClass()
        {
            var input = "[^abc]";
            ParseAndAcceptInput(input);
        }

        [TestMethod]
        public void RegexShouldParseOpenBracketInCharacterClass()
        {
            var input = "[\\]]";
            ParseAndAcceptInput(input);
        }
        
    }
}