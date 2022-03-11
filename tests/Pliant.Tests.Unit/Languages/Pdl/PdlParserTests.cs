using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Forest;
using Pliant.Languages.Pdl;
using Pliant.Languages.Regex;
using Pliant.Runtime;
using Pliant.Tests.Common.Forest;
using System;
using System.Text;

namespace Pliant.Tests.Unit.Languages.Pdl
{
    [TestClass]
    public class PdlParserTests
    {
        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void PdlParserShouldNotParseEmptyRule()
        {
            var pdl = Parse(@"");
            Assert.IsNotNull(pdl);
        }

        [TestMethod]
        public void PdlParserShouldParseCharacterProduction()
        {
            var expected = new PdlDefinition(
                    new PdlBlockRule(
                        new PdlRule(
                            new PdlQualifiedIdentifier("Rule"),
                            new PdlExpression(
                                new PdlTerm(
                                    new PdlFactorLiteral("a"))))));

            var actual = Parse(@"Rule = 'a';");

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void PdlParserShouldParseConcatenation()
        {
            var expected = new PdlDefinition(
                new PdlBlockRule(
                    new PdlRule(
                        new PdlQualifiedIdentifier("Rule"),
                        new PdlExpression(
                            new PdlTermConcatenation(
                                new PdlFactorLiteral("a"),
                                new PdlTerm(
                                    new PdlFactorLiteral("b")))))));

            var actual = Parse(@"Rule = 'a' 'b';");

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void PdlParserShouldParseAlteration()
        {
            var expected = new PdlDefinition(
                new PdlBlockRule(
                    new PdlRule(
                        new PdlQualifiedIdentifier("Rule"),
                        new PdlExpressionAlteration(
                            new PdlTerm(
                                new PdlFactorLiteral("a")),
                            new PdlExpression(
                                new PdlTerm(
                                    new PdlFactorLiteral("b")))))));
            var actual = Parse(@"Rule = 'a' | 'b';");
            Assert.AreEqual(expected, actual);

        }

        [TestMethod]
        public void PdlParserShouldParseAlterationAndConcatenation()
        {
            var expected = new PdlDefinition(
                new PdlBlockRule(
                    new PdlRule(
                        new PdlQualifiedIdentifier("Rule"),
                        new PdlExpressionAlteration(
                            new PdlTermConcatenation(
                                new PdlFactorLiteral("a"),
                                new PdlTerm(
                                    new PdlFactorLiteral("b"))),
                            new PdlExpression(
                                new PdlTerm(
                                    new PdlFactorLiteral("c")))))));
            var actual = Parse(@"Rule = 'a' 'b' | 'c';");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void PdlParserShouldParseRegularExpression()
        {
            var expected = new PdlDefinition(
                    new PdlBlockRule(
                        new PdlRule(
                            new PdlQualifiedIdentifier("Rule"),
                            new PdlExpression(
                                new PdlTerm(
                                    new PdlFactorRegex(
                                        new RegexDefinition(
                                            startsWith: false,
                                            expression: new RegexExpressionTerm(
                                                new RegexTerm(
                                                    new RegexFactor(
                                                        new RegexAtomSet(
                                                            new RegexSet(false,
                                                                new RegexCharacterClass(
                                                                    new RegexCharacterRange(
                                                                        new RegexCharacterClassCharacter('a'),
                                                                        new RegexCharacterClassCharacter('z')))))))),
                                            endsWith: false)))))));

            var actual = Parse(@"Rule = /[a-z]/;");

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void PdlParserShouldParseRepetition()
        {
            var actual = Parse(@"Rule = { 'a' };");

            var expected = new PdlDefinition(
                    new PdlBlockRule(
                        new PdlRule(
                            new PdlQualifiedIdentifier("Rule"),
                            new PdlExpression(
                                new PdlTerm(
                                    new PdlFactorRepetition(
                                        new PdlExpression(
                                            new PdlTerm(
                                                new PdlFactorLiteral("a")))))))));

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void PdlParserShouldParseOptional()
        {
            var actual = Parse(@"Rule = [ 'a' ];");

            var expected = new PdlDefinition(
                    new PdlBlockRule(
                        new PdlRule(
                            new PdlQualifiedIdentifier("Rule"),
                            new PdlExpression(
                                new PdlTerm(
                                    new PdlFactorOptional(
                                        new PdlExpression(
                                            new PdlTerm(
                                                new PdlFactorLiteral("a")))))))));

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void PdlParserShouldParseGrouping()
        {
            var actual = Parse(@"Rule = ('a');");

            var expected = new PdlDefinition(
                    new PdlBlockRule(
                        new PdlRule(
                            new PdlQualifiedIdentifier("Rule"),
                            new PdlExpression(
                                new PdlTerm(
                                    new PdlFactorGrouping(
                                        new PdlExpression(
                                            new PdlTerm(
                                                new PdlFactorLiteral("a")))))))));

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void PdlParserShouldParseNamespace()
        {
            var expected = new PdlDefinition(
                    new PdlBlockRule(
                        new PdlRule(
                            new PdlQualifiedIdentifierConcatenation("This",
                                new PdlQualifiedIdentifierConcatenation("Is",
                                    new PdlQualifiedIdentifierConcatenation("A",
                                        new PdlQualifiedIdentifierConcatenation("Namespace",
                                        new PdlQualifiedIdentifier("Rule"))))),
                            new PdlExpression(
                                new PdlTerm(
                                    new PdlFactorLiteral("a"))))));

            var actual = Parse(@"This.Is.A.Namespace.Rule = 'a'; ");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void PdlParserShouldParseMultipleRules()
        {
            var expected = new PdlDefinitionConcatenation(
                new PdlBlockRule(
                    new PdlRule(
                        new PdlQualifiedIdentifier("S"),
                        new PdlExpression(
                            new PdlTermConcatenation(
                                new PdlFactorIdentifier(
                                    new PdlQualifiedIdentifier("A")),
                                new PdlTerm(
                                    new PdlFactorIdentifier(
                                        new PdlQualifiedIdentifier("B"))))))),
                new PdlDefinitionConcatenation(
                    new PdlBlockRule(
                        new PdlRule(
                            new PdlQualifiedIdentifier("A"),
                            new PdlExpression(
                                new PdlTerm(
                                    new PdlFactorLiteral("a"))))),
                    new PdlDefinition(
                        new PdlBlockRule(
                            new PdlRule(
                                new PdlQualifiedIdentifier(
                                    "B"),
                                new PdlExpression(
                                    new PdlTerm(
                                        new PdlFactorLiteral("b"))))))));
            var actual = Parse(@"
                S = A B;
                A = 'a';
                B = 'b';
            ");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void PdlParserShouldParseSettings()
        {
            var actual = Parse(@"
                :ignore = whitespace; ");
            Assert.IsNotNull(actual);

            var expected = new PdlDefinition(
                new PdlBlockSetting(
                    new PdlSetting(
                        new PdlSettingIdentifier("ignore"),
                        new PdlQualifiedIdentifier("whitespace"))));

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void PdlParserShouldParseSettingAndRule()
        {
            var actual = Parse(@"
                whitespace ~ /[\s]+/;
                :ignore = whitespace;");

            Assert.IsNotNull(actual);

            var whiteSpaceRegex = new RegexDefinition(
                false,
                new RegexExpressionTerm(
                    new RegexTerm(
                        new RegexFactorIterator(
                            new RegexAtomSet(
                                new RegexSet(
                                    false,
                                    new RegexCharacterClass(
                                        new RegexCharacterUnitRange(
                                            new RegexCharacterClassCharacter('s', true))))),
                            RegexIterator.OneOrMany))),
                false);
            var expected =
                new PdlDefinitionConcatenation(
                    new PdlBlockLexerRule(
                        new PdlLexerRule(
                            new PdlQualifiedIdentifier("whitespace"),
                            new PdlLexerRuleExpression(
                                new PdlLexerRuleTerm(
                                    new PdlLexerRuleFactorRegex(
                                        whiteSpaceRegex))))),
                    new PdlDefinition(
                        new PdlBlockSetting(
                            new PdlSetting(
                                new PdlSettingIdentifier("ignore"),
                                new PdlQualifiedIdentifier("whitespace")))));
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void PdlParserShouldParseLexerRule()
        {

            var actual = Parse(@"
                b ~ 'b' ;");
            Assert.IsNotNull(actual);

            var expected = new PdlDefinition(
                block: new PdlBlockLexerRule(
                   lexerRule: new PdlLexerRule(
                       qualifiedIdentifier: new PdlQualifiedIdentifier("b"),
                       expression: new PdlLexerRuleExpression(
                            term: new PdlLexerRuleTerm(
                                factor: new PdlLexerRuleFactorLiteral("b"))))));

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void PdlParserShouldParseComplexGrammarWithRepeat()
        {
            var stringBuilder = new StringBuilder()
            //.AppendLine("file = ws directives ws ;")
            .AppendLine("file = \"1\" { \"2\" } \"1\";");
            //.AppendLine("directives = directive { ows directive };")
            //.AppendLine("directive = \"0\" | \"1\"; ");

            var actual = Parse(stringBuilder.ToString());

            var grammar = new PdlGrammar();
            var parseEngine = new ParseEngine(grammar, new ParseEngineOptions(optimizeRightRecursion: false));
            var parseRunner = new ParseRunner(parseEngine, stringBuilder.ToString());
            while (!parseRunner.EndOfStream())
            {
                if (!parseRunner.Read())
                    throw new Exception(
                        $"Unable to parse Pdl. Error at position {parseRunner.Position}.");
            }
            if (!parseEngine.IsAccepted())
                throw new Exception(
                    $"Unable to parse Pdl. Error at position {parseRunner.Position}");

            var parseForest = parseEngine.GetParseForestRootNode();
            var visitor = new LoggingForestNodeVisitor(Console.Out);
            parseForest.Accept(visitor);
        }

        private static PdlDefinition Parse(string input)
        {
            var pdlParser = new PdlParser();
            return pdlParser.Parse(input);
        }
    }
}
