using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Forest;
using Pliant.Languages.Regex;
using Pliant.Runtime;
using System.IO;

namespace Pliant.Tests.Unit.Languages.Regex
{
    /// <summary>
    /// Summary description for RegexParserTests
    /// </summary>
    [TestClass]
    public class RegexParserTests
    {
        public TestContext TestContext { get; set; }
        
        [TestMethod]
        public void RegexParserShouldParseSingleCharacter()
        {
            var regexParser = new RegexParser();
            var actual = regexParser.Parse("a");
            var expected = new RegexDefinition(
                false,
                new RegexExpressionTerm(
                    new RegexTerm(
                        new RegexFactor(
                            new RegexAtomCharacter(
                                new RegexCharacter(
                                'a'))))),
                false);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void RegexParserShouldParsePositiveSet()
        {
            var regexParser = new RegexParser();
            var actual = regexParser.Parse("[a]");
            var expected = new RegexDefinition(
                false,
                new RegexExpressionTerm(
                    new RegexTerm(
                        new RegexFactor(
                            new RegexAtomSet(
                                new RegexSet(false,
                                    new RegexCharacterClass(
                                        new RegexCharacterUnitRange(
                                            new RegexCharacterClassCharacter('a')))))))),
                false);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void RegexParserShouldParseNegativeSet()
        {
            var regexParser = new RegexParser();
            var actual = regexParser.Parse("[^a]");

            var expected = new RegexDefinition(
                false,
                new RegexExpressionTerm(
                    new RegexTerm(
                        new RegexFactor(
                            new RegexAtomSet(
                                new RegexSet(true,
                                    new RegexCharacterClass(
                                        new RegexCharacterUnitRange(
                                            new RegexCharacterClassCharacter('a')))))))),
                false);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void DELETEME()
        {
            var pattern = "[a-zA-Z0-9]";

            var grammar = new RegexGrammar();
            var parser = new ParseEngine(grammar, new ParseEngineOptions(optimizeRightRecursion :false));
            var lexer = new ParseRunner(parser, pattern);

            var leoParser = new ParseEngine(grammar, new ParseEngineOptions(optimizeRightRecursion: true));
            var leoLexer = new ParseRunner(leoParser, pattern);

            lexer.RunToEnd();
            leoLexer.RunToEnd();

            var textWriter = new StringWriter();
            var leoTextWriter = new StringWriter();

            var visitor = new LoggingForestNodeVisitor(textWriter);
            var leoVisitor = new LoggingForestNodeVisitor(leoTextWriter);

            parser.GetParseForestRootNode().Accept(visitor);
            leoParser.GetParseForestRootNode().Accept(leoVisitor);

            var str = textWriter.ToString();
            var leoStr = leoTextWriter.ToString();
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void RegexParserShouldParseMultipleRanges()
        {
            var regexParser = new RegexParser();
            var actual = regexParser.Parse("[a-zA-Z0-9]");
            var expected = new RegexDefinition(
                startsWith: false,
                endsWith: false,
                expression: new RegexExpressionTerm(
                    term: new RegexTerm(
                        factor: new RegexFactor(
                            atom: new RegexAtomSet(
                                set: new RegexSet(
                                    negate: false,
                                    characterClass: new RegexCharacterClassAlteration(
                                        characterClass: new RegexCharacterClassAlteration(
                                            characterClass: new RegexCharacterClass(
                                                characterRange: new RegexCharacterRange(
                                                    startCharacter: new RegexCharacterClassCharacter(value: '0'),
                                                    endCharacter: new RegexCharacterClassCharacter(value: '9'))),
                                            characterRange: new RegexCharacterRange(
                                                startCharacter: new RegexCharacterClassCharacter(value: 'A'),
                                                endCharacter: new RegexCharacterClassCharacter(value: 'Z'))),
                                        characterRange: new RegexCharacterRange(
                                            startCharacter: new RegexCharacterClassCharacter(value: 'a'),
                                            endCharacter: new RegexCharacterClassCharacter(value: 'z')))))))));
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void RegexParserShouldParseAlteration()
        {
            var regexParser = new RegexParser();
            var actual = regexParser.Parse("a|b");
            var expected = new RegexDefinition(
                startsWith: false,
                endsWith: false,
                expression: new RegexExpressionAlteration(
                    term: new RegexTerm(
                        factor: new RegexFactor(
                            atom: new RegexAtomCharacter(
                                character: new RegexCharacter('a')))),
                    expression: new RegexExpressionTerm(
                        term: new RegexTerm(
                            factor: new RegexFactor(
                                atom: new RegexAtomCharacter(
                                    character: new RegexCharacter('b')))))));
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void RegexParserShouldParseEscape()
        {
            var regexParser = new RegexParser();
            var actual = regexParser.Parse(@"\.");

            var expected = new RegexDefinition(
                false, 
                new RegexExpressionTerm(
                    new RegexTerm(
                        new RegexFactor(
                            new RegexAtomCharacter(
                                new RegexCharacter('.', true))))), 
                false);
            Assert.AreEqual(expected, actual);
        }
    }
}