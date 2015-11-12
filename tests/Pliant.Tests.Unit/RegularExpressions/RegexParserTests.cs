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
    }
}
