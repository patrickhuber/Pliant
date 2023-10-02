using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Forest;
using Pliant.Grammars;
using Pliant.Languages.Regex;
using Pliant.Runtime;
using System;

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
        public void RegexShouldFailOnEmptyGroup()
        {
            var input = "()";
            ParseAndNotAcceptInput(input);
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

        [TestMethod]
        public void RegexLeoAndClassicTreesShouldMatch()
        {
            var input = "[a-zA-Z0-9]";
            var parser = new ParseEngine(_regexGrammar, new ParseEngineOptions(optimizeRightRecursion: true));
            var scanner = new ParseRunner(parser, input);
            Assert.IsTrue(scanner.RunToEnd());
            var forest = parser.GetParseForestRootNode();
            forest.Accept(new LoggingForestNodeVisitor(Console.Out));
        }
        
    }
}