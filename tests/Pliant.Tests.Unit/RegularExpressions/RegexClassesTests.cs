using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.RegularExpressions;

namespace Pliant.Tests.Unit.RegularExpressions
{
    [TestClass]
    public class RegexClassesTests
    {
        [TestMethod]
        public void RegexToStringShouldCreateSingleCharacterString()
        {
            var regex = new Regex(
                false, 
                new RegexExpressionTerm(
                    new RegexTerm(
                        new RegexFactor(
                            new RegexAtomCharacter(
                                new RegexCharacter('a'))))), 
                false);
            Assert.AreEqual("a", regex.ToString());
        }

        [TestMethod]
        public void RegexToStringShouldCreateConcatenationString()
        {

            var regex = new Regex(
                false,
                new RegexExpressionTerm(
                    new RegexTermFactor(                        
                            new RegexFactor(
                                new RegexAtomCharacter(
                                    new RegexCharacter('a'))),
                            new RegexTerm(
                                new RegexFactor(
                                    new RegexAtomCharacter(
                                        new RegexCharacter('b')))))),
                false);
            Assert.AreEqual("ab", regex.ToString());
        }

        [TestMethod]
        public void RegexToStringShouldCreateAlterationString()
        {
            var regex = new Regex(
                false,
                new RegexExpressionAlteration(
                    new RegexTerm(
                        new RegexFactor(
                            new RegexAtomCharacter(
                                new RegexCharacter('a')))),
                    new RegexExpressionTerm(
                        new RegexTerm(
                            new RegexFactor(
                                new RegexAtomCharacter(
                                    new RegexCharacter('b')))))),
                false);
            Assert.AreEqual("a|b", regex.ToString());
        }

        [TestMethod]
        public void RegexToStringShouldCreateSetString()
        {
            var regex = new Regex(
                false,
                new RegexExpressionTerm(
                    new RegexTerm(
                        new RegexFactor(
                            new RegexAtomSet(
                                new RegexSet(
                                    false, 
                                    new RegexCharacterClass(
                                        new RegexCharacterUnitRange(
                                            new RegexCharacterClassCharacter('a')))))))),
                false);
            Assert.AreEqual("[a]", regex.ToString());
        }

        [TestMethod]
        public void RegexToStringShouldCreateRangeString()
        {
            var regex = new Regex(
                false, 
                new RegexExpressionTerm(
                    new RegexTerm(
                        new RegexFactor(
                            new RegexAtomSet(
                                new RegexSet(
                                    false, 
                                    new RegexCharacterClass(
                                        new RegexCharacterRange(
                                            new RegexCharacterClassCharacter('a'),
                                            new RegexCharacterClassCharacter('z')))))))), 
                false);
            Assert.AreEqual("[a-z]", regex.ToString());
        }


        [TestMethod]
        public void RegexToStringShouldCreateNegativeRangeString()
        {
            var regex = new Regex(
                false,
                new RegexExpressionTerm(
                    new RegexTerm(
                        new RegexFactor(
                            new RegexAtomSet(
                                new RegexSet(
                                    true,
                                    new RegexCharacterClass(
                                        new RegexCharacterRange(
                                            new RegexCharacterClassCharacter('a'),
                                            new RegexCharacterClassCharacter('z')))))))),
                false);
            Assert.AreEqual("[^a-z]", regex.ToString());
        }
    }
}
