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
            // S = 'a';
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
            // S = 'a';
            // S = 'b';
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
        public void EbnfGrammarGeneratorShouldCreateGrammarForProductionAlteration()
        {
            // S = 'a' | 'b';
            var definition = new EbnfDefinition(
                new EbnfBlockRule(
                    new EbnfRule(
                        new EbnfQualifiedIdentifier("S"),
                        new EbnfExpressionAlteration(
                            new EbnfTerm(
                                new EbnfFactorLiteral("a")),
                            new EbnfExpression(
                                new EbnfTerm(
                                    new EbnfFactorLiteral("d")))))));
            var grammar = GenerateGrammar(definition);
            Assert.IsNotNull(grammar);
            Assert.IsNotNull(grammar.Start);
            Assert.AreEqual(2, grammar.Productions.Count);
        }

        [TestMethod]
        public void EbnfGrammarGeneratorShouldCreateGrammarForMultipleProductionsWithAlterations()
        {
            // S = 'a' | 'd';
            // S = 'b' | 'c';
            var definition = new EbnfDefinitionConcatenation(
                new EbnfBlockRule(
                    new EbnfRule(
                        new EbnfQualifiedIdentifier("S"),
                        new EbnfExpressionAlteration(
                            new EbnfTerm(
                                new EbnfFactorLiteral("a")),
                            new EbnfExpression(
                                new EbnfTerm(
                                    new EbnfFactorLiteral("d")))))),
                new EbnfDefinition(
                    new EbnfBlockRule(
                        new EbnfRule(
                            new EbnfQualifiedIdentifier("S"),
                            new EbnfExpressionAlteration(
                                new EbnfTerm(
                                    new EbnfFactorLiteral("b")),
                                new EbnfExpression(
                                    new EbnfTerm(
                                        new EbnfFactorLiteral("c"))))))));
            var grammar = GenerateGrammar(definition);
            Assert.IsNotNull(grammar);
            Assert.IsNotNull(grammar.Start);
            Assert.AreEqual(4, grammar.Productions.Count);
        } 

        [TestMethod]
        public void EbnfGrammarGeneratorShouldCreateGrammarForRepetition()
        {
            // R = { 'a' } ;
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
            Assert.AreEqual(3, grammar.Productions.Count);

            Assert.AreEqual(2, grammar.Productions[0].RightHandSide.Count);
            Assert.AreEqual(0, grammar.Productions[1].RightHandSide.Count);
            Assert.AreEqual(1, grammar.Productions[2].RightHandSide.Count);
        }

        [TestMethod]
        public void EbnfGrammarGeneratorShouldCreateGrammarForGrouping()
        {
            // R = ( 'a' );
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
            Assert.AreEqual(2, grammar.Productions.Count);
            Assert.AreEqual(1, grammar.Productions[0].RightHandSide.Count);
            Assert.AreEqual(1, grammar.Productions[1].RightHandSide.Count);
        }


        [TestMethod]
        public void EbnfGrammarGeneratorShouldCreateGrammarForOptional()
        {
            // R = ['a']
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
            Assert.IsNotNull(grammar);
            Assert.IsNotNull(grammar.Start);
            
            ProductionBuilder R = "R";
            ProductionBuilder optA = "[a]";

            R.Definition = optA;
            optA.Definition = 'a'
                | (_)null;

            var expectedGrammar = new GrammarBuilder(R, new[] { R, optA }).ToGrammar();
            Assert.AreEqual(expectedGrammar.Productions.Count, grammar.Productions.Count);
        }

        [TestMethod]
        public void EbnfGrammarGeneratorShouldCreateGrammarForMultipleOptionals()
        {
            // R = 'b' ['a'] 'c' ['d']
            var definition = new EbnfDefinition(
                new EbnfBlockRule(
                    new EbnfRule(
                        new EbnfQualifiedIdentifier("R"),
                        new EbnfExpression(
                            new EbnfTermConcatenation(
                                new EbnfFactorLiteral("b"),
                                new EbnfTermConcatenation(
                                    new EbnfFactorOptional(
                                        new EbnfExpression(
                                            new EbnfTerm(
                                                new EbnfFactorLiteral("a")))),
                                    new EbnfTermConcatenation(
                                        new EbnfFactorLiteral("c"),
                                        new EbnfTerm(
                                            new EbnfFactorOptional(
                                                new EbnfExpression(
                                                    new EbnfTerm(
                                                        new EbnfFactorLiteral("d"))))))))))));
            var grammar = GenerateGrammar(definition);
            Assert.IsNotNull(grammar);
            Assert.IsNotNull(grammar.Start);

            ProductionBuilder R = "R";
            ProductionBuilder optA = "[a]";
            ProductionBuilder optD = "[d]";

            R.Definition =
                (_)'b' + optA+ 'c' + optD ;
            optA.Definition = 'a' | (_)null;
            optD.Definition = 'd' | (_)null;

            var expectedGrammar = new GrammarBuilder(R).ToGrammar();
            Assert.AreEqual(expectedGrammar.Productions.Count, grammar.Productions.Count);
        }

        private static IGrammar GenerateGrammar(EbnfDefinition definition)
        {
            var generator = new EbnfGrammarGenerator();
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
