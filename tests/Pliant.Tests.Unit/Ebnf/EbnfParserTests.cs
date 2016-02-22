using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Ebnf;
using Pliant.RegularExpressions;
using System;

namespace Pliant.Tests.Unit.Ebnf
{
    [TestClass]
    public class EbnfParserTests
    {
        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void EbnfParserShouldNotParseEmptyRule()
        {
            var ebnf = Parse(@"");
            Assert.IsNotNull(ebnf);
        }

        [TestMethod]
        public void EbnfParserShouldParseCharacterProduction()
        {
            var expected = new EbnfDefinition(
                    new EbnfBlockRule(
                        new EbnfRule(
                            new EbnfQualifiedIdentifier("Rule"),
                            new EbnfExpression(
                                new EbnfTerm(
                                    new EbnfFactorLiteral("a"))))));

            var actual = Parse(@"Rule = 'a';");

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void EbnfParserShouldParseConcatenation()
        {
            var expected = new EbnfDefinition(
                new EbnfBlockRule(
                    new EbnfRule(
                        new EbnfQualifiedIdentifier("Rule"),
                        new EbnfExpression(
                            new EbnfTermRepetition(
                                new EbnfFactorLiteral("a"),
                                new EbnfTerm(
                                    new EbnfFactorLiteral("b")))))));

            var actual =  Parse(@"Rule = 'a' 'b';");

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void EbnfParserShouldParseAlteration()
        {
            var expected = new EbnfDefinition(
                new EbnfBlockRule(
                    new EbnfRule(
                        new EbnfQualifiedIdentifier("Rule"),
                        new EbnfExpressionAlteration(
                            new EbnfTerm(
                                new EbnfFactorLiteral("a")),
                            new EbnfExpression(
                                new EbnfTerm(
                                    new EbnfFactorLiteral("b")))))));
            var actual =  Parse(@"Rule = 'a' | 'b';");
            Assert.AreEqual(expected, actual);

        }

        [TestMethod]
        public void EbnfParserShouldParseAlterationAndConcatenation()
        {
            var expected = new EbnfDefinition(
                new EbnfBlockRule(
                    new EbnfRule(
                        new EbnfQualifiedIdentifier("Rule"),
                        new EbnfExpressionAlteration(
                            new EbnfTermRepetition(
                                new EbnfFactorLiteral("a"),
                                new EbnfTerm(
                                    new EbnfFactorLiteral("b"))),
                            new EbnfExpression(
                                new EbnfTerm(
                                    new EbnfFactorLiteral("c")))))));
            var actual =  Parse(@"Rule = 'a' 'b' | 'c';");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void EbnfParserRegularExpression()
        {
            var expected = new EbnfDefinition(
                    new EbnfBlockRule(
                        new EbnfRule(
                            new EbnfQualifiedIdentifier("Rule"),
                            new EbnfExpression(
                                new EbnfTerm(
                                    new EbnfFactorRegex(
                                        new Regex(
                                            startsWith: false, 
                                            expression: new RegexExpressionTerm(
                                                new RegexTerm(
                                                    new RegexFactor(
                                                        new RegexAtomSet(
                                                            new RegexSet(false, 
                                                                new RegexCharacterClass(
                                                                    new RegexCharacterRangeSet(
                                                                        new RegexCharacterClassCharacter('a'),
                                                                        new RegexCharacterClassCharacter('z')))))))), 
                                            endsWith: false)))))));

            var actual = Parse(@"Rule = /[a-z]/;");

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void EbnfParserShouldParseRepetition()
        {
            var actual =  Parse(@"Rule = { 'a' };");

            var expected = new EbnfDefinition(
                    new EbnfBlockRule(
                        new EbnfRule(
                            new EbnfQualifiedIdentifier("Rule"),
                            new EbnfExpression(
                                new EbnfTerm(
                                    new EbnfFactorRepetition(
                                        new EbnfExpression(
                                            new EbnfTerm(
                                                new EbnfFactorLiteral("a")))))))));

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void EbnfParserShouldParseOptional()
        {
            var actual =  Parse(@"Rule = [ 'a' ];");

            var expected = new EbnfDefinition(
                    new EbnfBlockRule(
                        new EbnfRule(
                            new EbnfQualifiedIdentifier("Rule"),
                            new EbnfExpression(
                                new EbnfTerm(
                                    new EbnfFactorOptional(
                                        new EbnfExpression(
                                            new EbnfTerm(
                                                new EbnfFactorLiteral("a")))))))));

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void EbnfParserShouldParseGrouping()
        {
            var actual =  Parse(@"Rule = ('a');");

            var expected = new EbnfDefinition(
                    new EbnfBlockRule(
                        new EbnfRule(
                            new EbnfQualifiedIdentifier("Rule"),
                            new EbnfExpression(
                                new EbnfTerm(
                                    new EbnfFactorGrouping(
                                        new EbnfExpression(
                                            new EbnfTerm(
                                                new EbnfFactorLiteral("a")))))))));

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void EbnfParserShouldParseNamespace()
        {
            var expected = new EbnfDefinition(
                    new EbnfBlockRule(
                        new EbnfRule(
                            new EbnfQualifiedIdentifierRepetition("This",
                                new EbnfQualifiedIdentifierRepetition("Is",
                                    new EbnfQualifiedIdentifierRepetition("A",
                                        new EbnfQualifiedIdentifierRepetition("Namespace",
                                        new EbnfQualifiedIdentifier("Rule"))))),
                            new EbnfExpression(
                                new EbnfTerm(
                                    new EbnfFactorLiteral("a"))))));

            var actual = Parse(@"This.Is.A.Namespace.Rule = 'a'; ");
            Assert.AreEqual(expected, actual);
        }

        private static EbnfDefinition Parse(string input)
        {
            var ebnfParser = new EbnfParser();
            return ebnfParser.Parse(input);
        }
    }
}
