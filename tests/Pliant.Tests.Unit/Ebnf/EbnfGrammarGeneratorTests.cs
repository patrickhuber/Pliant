using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Builders;
using Pliant.Ebnf;
using Pliant.Grammars;

namespace Pliant.Tests.Unit.Ebnf
{
    [TestClass]
    public class EbnfGrammarGeneratorTests
    {
        [TestMethod]
        public void EbnfGrammarGeneratorShouldCreateGrammarForSimpleRule()
        {
            var definition = new EbnfDefinition(
                new EbnfBlockRule(
                    new EbnfRule(
                        new EbnfQualifiedIdentifier("S"),
                        new EbnfExpression(
                            new EbnfTerm(
                                new EbnfFactorLiteral("a"))))));
            var grammar = GenerateGrammar(definition);
            Assert.IsNotNull(grammar);
            Assert.IsNotNull(grammar.Start);
            Assert.AreEqual(1, grammar.Productions.Count);
            Assert.AreEqual(1, grammar.Productions[0].RightHandSide.Count);
        }

        [TestMethod]
        public void EbnfGrammarGeneratorShouldCreateGrammarForMultipleProductions()
        {
            var definition = new EbnfDefinitionConcatenation(                
                new EbnfBlockRule(
                    new EbnfRule(
                        new EbnfQualifiedIdentifier("S"),
                        new EbnfExpression(
                            new EbnfTerm(
                                new EbnfFactorLiteral("a"))))),
                new EbnfDefinition(
                    new EbnfBlockRule(
                        new EbnfRule(
                            new EbnfQualifiedIdentifier("S"),
                            new EbnfExpression(
                                new EbnfTerm(
                                    new EbnfFactorLiteral("b")))))));
            var grammar = GenerateGrammar(definition);
            Assert.IsNotNull(grammar);
            Assert.IsNotNull(grammar.Start);
            Assert.AreEqual(2, grammar.Productions.Count);
        }
                
        [TestMethod]
        public void EbnfGrammarGeneratorShouldCreateGrammarForRepetition()
        {
            var definition = new EbnfDefinition(
                new EbnfBlockRule(
                    new EbnfRule(
                        new EbnfQualifiedIdentifier("R"),
                        new EbnfExpression(
                            new EbnfTerm(
                                new EbnfFactorRepetition(
                                    new EbnfExpression(
                                        new EbnfTerm(
                                            new EbnfFactorLiteral("a")))))))));

            var grammar = GenerateGrammar(definition);
            Assert.IsNotNull(grammar);
            Assert.IsNotNull(grammar.Start);
            Assert.AreEqual(1, grammar.Productions.Count);
            Assert.AreEqual(1, grammar.Productions[0].RightHandSide.Count);
        }

        [TestMethod]
        public void EbnfGrammarGeneratorShouldCreateGrammarForGrouping()
        {
            var definition = new EbnfDefinition(
                new EbnfBlockRule(
                    new EbnfRule(
                        new EbnfQualifiedIdentifier("R"),
                        new EbnfExpression(
                            new EbnfTerm(
                                new EbnfFactorGrouping(
                                    new EbnfExpression(
                                        new EbnfTerm(
                                            new EbnfFactorLiteral("a")))))))));

            var grammar = GenerateGrammar(definition);
            Assert.IsNotNull(grammar);
            Assert.IsNotNull(grammar.Start);
            Assert.AreEqual(1, grammar.Productions.Count);
            Assert.AreEqual(1, grammar.Productions[0].RightHandSide.Count);
        }


        [TestMethod]
        public void EbnfGrammarGeneratorShouldCreateGrammarForOptional()
        {
            var definition = new EbnfDefinition(
                new EbnfBlockRule(
                    new EbnfRule(
                        new EbnfQualifiedIdentifier("R"),
                        new EbnfExpression(
                            new EbnfTerm(
                                new EbnfFactorOptional(
                                    new EbnfExpression(
                                        new EbnfTerm(
                                            new EbnfFactorLiteral("a")))))))));

            var grammar = GenerateGrammar(definition);

            ProductionBuilder R = "R";
            R.Definition = 'a' 
                |   (_)null;           
            var expectedGrammar = new GrammarBuilder(R, new[] { R }).ToGrammar();
            Assert.IsNotNull(grammar);
            Assert.IsNotNull(grammar.Start);
            Assert.AreEqual(expectedGrammar.Productions.Count, grammar.Productions.Count);

        }

        private static IGrammar GenerateGrammar(EbnfDefinition definition)
        {
            var strategy = new GuidEbnfProductionNamingStrategy();
            var generator = new EbnfGrammarGenerator(strategy);
            return generator.Generate(definition);
        }

        private class GuidEbnfProductionNamingStrategy : IEbnfProductionNamingStrategy
        {
            public INonTerminal GetSymbolForOptional(EbnfFactorOptional optional)
            {
                return new NonTerminal(Guid.NewGuid().ToString());
            }

            public INonTerminal GetSymbolForRepetition(EbnfFactorRepetition repetition)
            {
                return new NonTerminal(Guid.NewGuid().ToString());
            }
        }
    }
}
