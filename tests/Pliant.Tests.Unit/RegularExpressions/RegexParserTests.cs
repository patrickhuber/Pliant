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
            var regex = regexParser.Parse("a");

            // regex object validation
            Assert.IsNotNull(regex);
            Assert.IsFalse(regex.StartsWith);
            Assert.IsFalse(regex.EndsWith);

            // there should be a expression
            Assert.IsNotNull(regex.Expression);
            Assert.IsInstanceOfType(regex.Expression, typeof(RegexExpressionTerm));
            var expression = regex.Expression as RegexExpressionTerm;

            // which should have a term
            Assert.IsInstanceOfType(expression.Term, typeof(RegexTerm));

            // which should have a factor
            Assert.IsInstanceOfType(expression.Term.Factor, typeof(RegexFactor));

            // which should have an atom
            Assert.IsInstanceOfType(expression.Term.Factor.Atom, typeof(RegexAtomCharacter));
            var atom = expression.Term.Factor.Atom as RegexAtomCharacter;

            // which should have a character value of 'a'
            Assert.AreEqual('a', atom.Character.Value);
        }

        [TestMethod]
        public void Test_RegexParser_That_Positive_Set_Returns_Proper_Object()
        {
            var regexParser = new RegexParser();
            var regex = regexParser.Parse("[a]");

            // regex object validation
            Assert.IsNotNull(regex);
            Assert.IsFalse(regex.StartsWith);
            Assert.IsFalse(regex.EndsWith);

            // there should be a expression
            Assert.IsNotNull(regex.Expression);
            Assert.IsInstanceOfType(regex.Expression, typeof(RegexExpressionTerm));
            var expression = regex.Expression as RegexExpressionTerm;

            // which should have a term
            Assert.IsInstanceOfType(expression.Term, typeof(RegexTerm));

            // which should have a factor
            Assert.IsInstanceOfType(expression.Term.Factor, typeof(RegexFactor));

            // which should have an atom
            Assert.IsInstanceOfType(expression.Term.Factor.Atom, typeof(RegexAtomSet));
            var atom = expression.Term.Factor.Atom as RegexAtomSet;

            // which should have a positive set
            Assert.IsInstanceOfType(atom.Set, typeof(RegexSet));
            Assert.IsFalse(atom.Set.Negate);

            // which should have a character class
            Assert.IsInstanceOfType(atom.Set.CharacterClass, typeof(RegexCharacterClass));
            var characterClass = atom.Set.CharacterClass;

            // which should have a character range
            Assert.IsInstanceOfType(characterClass.CharacterRange, typeof(RegexCharacterRange));

            // which should have a character class character
            Assert.IsInstanceOfType(characterClass.CharacterRange.StartCharacter, typeof(RegexCharacterClassCharacter));

            // which should have a value of 'a'
            Assert.AreEqual('a', characterClass.CharacterRange.StartCharacter.Value);
        }

        [TestMethod]
        public void Test_RegexParser_That_Parses_SubExpression()
        {
            var regexParser = new RegexParser();
            var regex = regexParser.Parse("(()())()()");
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
