using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            Assert.IsNotNull(grammar);
        }

        private static IGrammar GenerateGrammar(EbnfDefinition definition)
        {
            return new EbnfGrammarGenerator().Generate(definition);
        }
    }
}
