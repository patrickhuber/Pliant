using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Ebnf;
using Pliant.RegularExpressions;
using Pliant.Runtime;
using Pliant.Tests.Common.Forest;
using System;
using System.Text;

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
                            new EbnfTermConcatenation(
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
                            new EbnfTermConcatenation(
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
        public void EbnfParserShouldParseRegularExpression()
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
                                                                    new RegexCharacterRange(
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
                            new EbnfQualifiedIdentifierConcatenation("This",
                                new EbnfQualifiedIdentifierConcatenation("Is",
                                    new EbnfQualifiedIdentifierConcatenation("A",
                                        new EbnfQualifiedIdentifierConcatenation("Namespace",
                                        new EbnfQualifiedIdentifier("Rule"))))),
                            new EbnfExpression(
                                new EbnfTerm(
                                    new EbnfFactorLiteral("a"))))));

            var actual = Parse(@"This.Is.A.Namespace.Rule = 'a'; ");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void EbnfParserShouldParseMultipleRules()
        {
            var expected = new EbnfDefinitionConcatenation(
                new EbnfBlockRule(
                    new EbnfRule(
                        new EbnfQualifiedIdentifier("S"),
                        new EbnfExpression(
                            new EbnfTermConcatenation(
                                new EbnfFactorIdentifier(
                                    new EbnfQualifiedIdentifier("A")),
                                new EbnfTerm(
                                    new EbnfFactorIdentifier(
                                        new EbnfQualifiedIdentifier("B"))))))),
                new EbnfDefinitionConcatenation(
                    new EbnfBlockRule(
                        new EbnfRule(
                            new EbnfQualifiedIdentifier("A"),
                            new EbnfExpression(
                                new EbnfTerm(
                                    new EbnfFactorLiteral("a"))))),
                    new EbnfDefinition(
                        new EbnfBlockRule(
                            new EbnfRule(
                                new EbnfQualifiedIdentifier(
                                    "B"),
                                new EbnfExpression(
                                    new EbnfTerm(
                                        new EbnfFactorLiteral("b"))))))));
            var actual = Parse(@"
                S = A B;
                A = 'a';
                B = 'b';
            ");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void EbnfParserShouldParseSettings()
        {
            var actual = Parse(@"
                :ignore = whitespace; ");
            Assert.IsNotNull(actual);

            var expected = new EbnfDefinition(
                new EbnfBlockSetting(
                    new EbnfSetting(
                        new EbnfSettingIdentifier(":ignore"),
                        new EbnfQualifiedIdentifier("whitespace"))));

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void EbnfParserShouldParseLexerRule()
        {
            
            var actual = Parse(@"
                b ~ 'b' ;");
            Assert.IsNotNull(actual);

            var expected = new EbnfDefinition(
                block: new EbnfBlockLexerRule(
                   lexerRule: new EbnfLexerRule(
                       qualifiedIdentifier: new EbnfQualifiedIdentifier("b"),
                       expression:  new EbnfLexerRuleExpression(
                            term: new EbnfLexerRuleTerm(
                                factor: new EbnfLexerRuleFactorLiteral("b"))))));

            Assert.AreEqual(expected, actual);
        }
        
        [TestMethod]
        public void EbnfParserShouldParseComplexGrammarWithRepeat()
        {
            var stringBuilder = new StringBuilder()
            //.AppendLine("file = ws directives ws ;")
            .AppendLine("file = \"1\" { \"2\" } \"1\";");
            //.AppendLine("directives = directive { ows directive };")
            //.AppendLine("directive = \"0\" | \"1\"; ");

            var actual = Parse(stringBuilder.ToString());

            var grammar = new EbnfGrammar();
            var parseEngine = new ParseEngine(grammar, new ParseEngineOptions(optimizeRightRecursion: false));
            var parseRunner = new ParseRunner(parseEngine, stringBuilder.ToString());
            while (!parseRunner.EndOfStream())
            {
                if (!parseRunner.Read())
                    throw new Exception(
                        $"Unable to parse Ebnf. Error at position {parseRunner.Position}.");
            }
            if (!parseEngine.IsAccepted())
                throw new Exception(
                    $"Unable to parse Ebnf. Error at position {parseRunner.Position}");

            var parseForest = parseEngine.GetParseForestRootNode();
            var visitor = new LoggingForestNodeVisitor(Console.Out);
            parseForest.Accept(visitor);
        }

        private static EbnfDefinition Parse(string input)
        {
            var ebnfParser = new EbnfParser();
            return ebnfParser.Parse(input);
        }
    }
}
