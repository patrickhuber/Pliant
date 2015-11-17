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
        public void Test_RegexParser_That_Single_Character_Returns_Proper_Object()
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
        public void Test_RegexParser_That_Positive_Set_Returns_Proper_Object()
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
                                        new RegexCharacterRange(
                                            new RegexCharacterClassCharacter('a')))))))), 
                false);
            Assert.AreEqual(expected, actual);            
        }

        [TestMethod]
        public void Test_RegexParser_That_Parses_SubExpression()
        {
            var regexParser = new RegexParser();
            var actual = regexParser.Parse("(()())()()");
            var expected = new Regex(
                false, 
                new RegexExpressionTerm(
                    new RegexTermFactor(
                        new RegexFactor(
                            new RegexAtomExpression(
                                new RegexExpressionTerm(
                                    new RegexTermFactor(
                                        new RegexFactor(
                                            new RegexAtomExpression(
                                                new RegexExpression())),
                                        new RegexTerm(
                                            new RegexFactor(new RegexAtomExpression(
                                                new RegexExpression()))))))),
                        new RegexTermFactor(
                            new RegexFactor(
                                new RegexAtomExpression(
                                    new RegexExpression())),
                            new RegexTerm(
                                new RegexFactor(
                                    new RegexAtomExpression(
                                        new RegexExpression())))))),
                false);
            Assert.AreEqual(actual, expected);
        }

        [TestMethod]
        public void Test_RegexParser_That_Parses_Multiple_Ranges()
        {
            var regexParser = new RegexParser();
            var actual = regexParser.Parse("[a-zA-Z0-9]");
            var expected = new Regex(
                startsWith :false, 
                endsWith:false, 
                expression: new RegexExpressionTerm(
                    term:  new RegexTerm(
                        factor: new RegexFactor(
                            atom: new RegexAtomSet(
                                set: new RegexSet(
                                    negate: false, 
                                    characterClass: new RegexCharacterClassList(
                                        characterClass: new RegexCharacterClassList(
                                            characterClass: new RegexCharacterClass(
                                                characterRange: new RegexCharacterRangeSet(
                                                    startCharacter:  new RegexCharacterClassCharacter(value: '0' ),
                                                    endCharacter: new RegexCharacterClassCharacter (value: '9' ))),
                                            characterRange: new RegexCharacterRangeSet(
                                                startCharacter: new RegexCharacterClassCharacter(value: 'A' ),
                                                endCharacter: new RegexCharacterClassCharacter(value: 'Z' ))),
                                        characterRange: new RegexCharacterRangeSet(
                                            startCharacter: new RegexCharacterClassCharacter(value: 'a' ),
                                            endCharacter: new RegexCharacterClassCharacter(value: 'z' )))))))));
            Assert.AreEqual(expected, actual);
        }
    }
}
