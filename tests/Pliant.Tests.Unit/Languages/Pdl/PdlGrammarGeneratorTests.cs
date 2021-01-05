using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Languages.Pdl;
using Pliant.Grammars;
using Pliant.Builders.Expressions;
using Pliant.Runtime;
using Pliant.Tests.Common;
using Pliant.Languages.Regex;

namespace Pliant.Tests.Unit.Languages.Pdl
{
    [TestClass]
    public class PdlGrammarGeneratorTests
    {
        [TestMethod]
        public void PdlGrammarGeneratorShouldCreateGrammarForSimpleRule()
        {
            // S = 'a';
            var definition = new PdlDefinition(
                new PdlBlockRule(
                    new PdlRule(
                        new PdlQualifiedIdentifier("S"),
                        new PdlExpression(
                            new PdlTerm(
                                new PdlFactorLiteral("a"))))));
            var grammar = GenerateGrammar(definition);
            Assert.IsNotNull(grammar);
            Assert.IsNotNull(grammar.Start);
            Assert.AreEqual(1, grammar.Productions.Count);
            Assert.AreEqual(1, grammar.Productions[0].RightHandSide.Count);
        }

        [TestMethod]
        public void PdlGrammarGeneratorShouldCreateGrammarForMultipleProductions()
        {
            // S = 'a';
            // S = 'b';
            var definition = new PdlDefinitionConcatenation(                
                new PdlBlockRule(
                    new PdlRule(
                        new PdlQualifiedIdentifier("S"),
                        new PdlExpression(
                            new PdlTerm(
                                new PdlFactorLiteral("a"))))),
                new PdlDefinition(
                    new PdlBlockRule(
                        new PdlRule(
                            new PdlQualifiedIdentifier("S"),
                            new PdlExpression(
                                new PdlTerm(
                                    new PdlFactorLiteral("b")))))));
            var grammar = GenerateGrammar(definition);
            Assert.IsNotNull(grammar);
            Assert.IsNotNull(grammar.Start);
            Assert.AreEqual(2, grammar.Productions.Count);
        }

        [TestMethod]
        public void PdlGrammarGeneratorShouldCreateGrammarForProductionAlteration()
        {
            // S = 'a' | 'b';
            var definition = new PdlDefinition(
                new PdlBlockRule(
                    new PdlRule(
                        new PdlQualifiedIdentifier("S"),
                        new PdlExpressionAlteration(
                            new PdlTerm(
                                new PdlFactorLiteral("a")),
                            new PdlExpression(
                                new PdlTerm(
                                    new PdlFactorLiteral("d")))))));
            var grammar = GenerateGrammar(definition);
            Assert.IsNotNull(grammar);
            Assert.IsNotNull(grammar.Start);
            Assert.AreEqual(2, grammar.Productions.Count);
        }

        [TestMethod]
        public void PdlGrammarGeneratorShouldCreateGrammarForMultipleProductionsWithAlterations()
        {
            // S = 'a' | 'd';
            // S = 'b' | 'c';
            var definition = new PdlDefinitionConcatenation(
                new PdlBlockRule(
                    new PdlRule(
                        new PdlQualifiedIdentifier("S"),
                        new PdlExpressionAlteration(
                            new PdlTerm(
                                new PdlFactorLiteral("a")),
                            new PdlExpression(
                                new PdlTerm(
                                    new PdlFactorLiteral("d")))))),
                new PdlDefinition(
                    new PdlBlockRule(
                        new PdlRule(
                            new PdlQualifiedIdentifier("S"),
                            new PdlExpressionAlteration(
                                new PdlTerm(
                                    new PdlFactorLiteral("b")),
                                new PdlExpression(
                                    new PdlTerm(
                                        new PdlFactorLiteral("c"))))))));
            var grammar = GenerateGrammar(definition);
            Assert.IsNotNull(grammar);
            Assert.IsNotNull(grammar.Start);
            Assert.AreEqual(4, grammar.Productions.Count);
        } 

        [TestMethod]
        public void PdlGrammarGeneratorShouldCreateGrammarForRepetition()
        {
            // R = { 'a' } ;
            var definition = new PdlDefinition(
                new PdlBlockRule(
                    new PdlRule(
                        new PdlQualifiedIdentifier("R"),
                        new PdlExpression(
                            new PdlTerm(
                                new PdlFactorRepetition(
                                    new PdlExpression(
                                        new PdlTerm(
                                            new PdlFactorLiteral("a")))))))));

            var grammar = GenerateGrammar(definition);
            Assert.IsNotNull(grammar);
            Assert.IsNotNull(grammar.Start);
            Assert.AreEqual(3, grammar.Productions.Count);

            Assert.AreEqual(2, grammar.Productions[0].RightHandSide.Count);
            Assert.AreEqual(0, grammar.Productions[1].RightHandSide.Count);
            Assert.AreEqual(1, grammar.Productions[2].RightHandSide.Count);
        }

        [TestMethod]
        public void PdlGrammarGeneratorShouldCreateGrammarForGrouping()
        {
            // R = ( 'a' );
            var definition = new PdlDefinition(
                new PdlBlockRule(
                    new PdlRule(
                        new PdlQualifiedIdentifier("R"),
                        new PdlExpression(
                            new PdlTerm(
                                new PdlFactorGrouping(
                                    new PdlExpression(
                                        new PdlTerm(
                                            new PdlFactorLiteral("a")))))))));

            var grammar = GenerateGrammar(definition);
            Assert.IsNotNull(grammar);
            Assert.IsNotNull(grammar.Start);
            Assert.AreEqual(2, grammar.Productions.Count);
            Assert.AreEqual(1, grammar.Productions[0].RightHandSide.Count);
            Assert.AreEqual(1, grammar.Productions[1].RightHandSide.Count);
        }


        [TestMethod]
        public void PdlGrammarGeneratorShouldCreateGrammarForOptional()
        {
            // R = ['a']
            var definition = new PdlDefinition(
                new PdlBlockRule(
                    new PdlRule(
                        new PdlQualifiedIdentifier("R"),
                        new PdlExpression(
                            new PdlTerm(
                                new PdlFactorOptional(
                                    new PdlExpression(
                                        new PdlTerm(
                                            new PdlFactorLiteral("a")))))))));

            var grammar = GenerateGrammar(definition);
            Assert.IsNotNull(grammar);
            Assert.IsNotNull(grammar.Start);
            
            ProductionExpression 
                R = "R",
                optA = "[a]";

            R.Rule = optA;
            optA.Rule = 'a'
                | (Expr)null;

            var expectedGrammar = new GrammarExpression(R, new[] { R, optA }).ToGrammar();
            Assert.AreEqual(expectedGrammar.Productions.Count, grammar.Productions.Count);
        }

        [TestMethod]
        public void PdlGrammarGeneratorShouldCreateGrammarForMultipleOptionals()
        {
            // R = 'b' ['a'] 'c' ['d']
            var definition = new PdlDefinition(
                new PdlBlockRule(
                    new PdlRule(
                        new PdlQualifiedIdentifier("R"),
                        new PdlExpression(
                            new PdlTermConcatenation(
                                new PdlFactorLiteral("b"),
                                new PdlTermConcatenation(
                                    new PdlFactorOptional(
                                        new PdlExpression(
                                            new PdlTerm(
                                                new PdlFactorLiteral("a")))),
                                    new PdlTermConcatenation(
                                        new PdlFactorLiteral("c"),
                                        new PdlTerm(
                                            new PdlFactorOptional(
                                                new PdlExpression(
                                                    new PdlTerm(
                                                        new PdlFactorLiteral("d"))))))))))));
            var grammar = GenerateGrammar(definition);
            Assert.IsNotNull(grammar);
            Assert.IsNotNull(grammar.Start);

            ProductionExpression 
                R = "R",
                optA = "[a]",
                optD = "[d]";

            R.Rule =
                (Expr)'b' + optA + 'c' + optD ;
            optA.Rule = 'a' | (Expr)null;
            optD.Rule = 'd' | (Expr)null;

            var expectedGrammar = new GrammarExpression(R, new[] { R, optA, optD }).ToGrammar();
            Assert.AreEqual(expectedGrammar.Productions.Count, grammar.Productions.Count);
        }

        [TestMethod]
        public void PdlGrammarGeneratorShouldCreateGrammarForComplexDefinition()
        {
            var ebnf = 
                @"file = ws directives ws ;
                ws = [ ows ] ; /* white space */
                ows = ""_""; /* obligatory white space */
                directives = directive { ows directive };
                directive = ""0"" | ""1""; ";

            var parser = new PdlParser();
            var ebnfDefinition = parser.Parse(ebnf);
            var generatedGrammar = GenerateGrammar(ebnfDefinition);
            Assert.IsNotNull(generatedGrammar);
            var parseEngine = new ParseEngine(generatedGrammar, new ParseEngineOptions(optimizeRightRecursion: true ));
            var parseTester = new ParseTester(parseEngine);
            parseTester.RunParse("_0_1_0_0_1_1_");
        }


        [TestMethod]
        public void PdlGeneratorShouldGenerateIgnores()
        {
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
                                            new RegexCharacterClassCharacter(' '))))),
                            RegexIterator.OneOrMany))),
                false);
            var definition = new PdlDefinitionConcatenation(
                new PdlBlockRule(
                    new PdlRule(
                        new PdlQualifiedIdentifier("S"),
                        new PdlExpression(
                            new PdlTerm(
                                new PdlFactorLiteral("a"))))),
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
                                new PdlQualifiedIdentifier("whitespace"))))));
            
            var grammar = GenerateGrammar(definition);
            Assert.IsNotNull(grammar.Ignores);
            Assert.AreEqual(1, grammar.Ignores.Count);
        }

        [TestMethod]
        public void PdlGeneratorShouldGenerateTrivia()
        {
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
                                            new RegexCharacterClassCharacter(' '))))),
                            RegexIterator.OneOrMany))),
                false);
            var definition = new PdlDefinitionConcatenation(
                new PdlBlockRule(
                    new PdlRule(
                        new PdlQualifiedIdentifier("S"),
                        new PdlExpression(
                            new PdlTerm(
                                new PdlFactorLiteral("a"))))),
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
                                new PdlSettingIdentifier("trivia"),
                                new PdlQualifiedIdentifier("whitespace"))))));

            var grammar = GenerateGrammar(definition);
            Assert.IsNotNull(grammar.Trivia);
            Assert.AreEqual(1, grammar.Trivia.Count);
        }

        [TestMethod]
        public void PdlGrammarGeneratorStartSettingShouldSetStartProduction()
        {
            // :start S;
            // S = 'a';
            var definition = new PdlDefinitionConcatenation(
                new PdlBlockSetting(
                    new PdlSetting(
                        new PdlSettingIdentifier("start"),
                        new PdlQualifiedIdentifier("S"))), 
                new PdlDefinition(
                new PdlBlockRule(
                    new PdlRule(
                        new PdlQualifiedIdentifier("S"),
                        new PdlExpression(
                            new PdlTerm(
                                new PdlFactorLiteral("a")))))));


            var grammar = GenerateGrammar(definition);
            Assert.IsNotNull(grammar.Start);
            Assert.AreEqual(grammar.Start.FullyQualifiedName.Name, "S");
        }

        private static IGrammar GenerateGrammar(PdlDefinition definition)
        {
            var generator = new PdlGrammarGenerator();
            return generator.Generate(definition);
        }

        private class GuidPdlProductionNamingStrategy : IPdlProductionNamingStrategy
        {
            public INonTerminal GetSymbolForOptional(PdlFactorOptional optional)
            {
                return new NonTerminal(Guid.NewGuid().ToString());
            }

            public INonTerminal GetSymbolForRepetition(PdlFactorRepetition repetition)
            {
                return new NonTerminal(Guid.NewGuid().ToString());
            }
        }
    }
}
