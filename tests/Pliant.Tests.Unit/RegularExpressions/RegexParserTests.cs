using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.RegularExpressions;

namespace Pliant.Tests.Unit.RegularExpressions
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
            var expected = new Regex(
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
            var expected = new Regex(
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

            var expected = new Regex(
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
        public void RegexParserShouldParseMultipleRanges()
        {
            var regexParser = new RegexParser();
            var actual = regexParser.Parse("[a-zA-Z0-9]");
            var expected = new Regex(
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
            var expected = new Regex(
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

            var expected = new Regex(
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