using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Ebnf;
using Pliant.Grammars;

namespace Pliant.Tests.Unit.Ebnf
{

    [TestClass]
    public class EbnfCompilerTests
    {
        [TestMethod]
        public void Test_EbnfCompiler_Creates_Character_Grammar_With_One_Production()
        {
            var grammar = Compile(@"Rule = 'a';");

            Assert.AreEqual("Rule", grammar.Start.Value);
            Assert.AreEqual(1, grammar.Productions.Count);

            var production = grammar.Productions[0];
            Assert.AreEqual(1, production.RightHandSide.Count);

            var symbol = production.RightHandSide[0];
            Assert.IsInstanceOfType(symbol, typeof(StringLiteralLexerRule));

            var lexerRule = symbol as StringLiteralLexerRule;
            Assert.AreEqual("a", lexerRule.Literal);
        }

        [TestMethod]
        public void Test_EbnfCompiler_Creates_Contactenation_With_One_Production()
        {
            var grammar = Compile( @"Rule = 'a' 'b';");

            Assert.AreEqual("Rule", grammar.Start.Value);
            Assert.AreEqual(1, grammar.Productions.Count);

            var production = grammar.Productions[0];
            Assert.AreEqual(2, production.RightHandSide.Count);

            var firstSymbol = production.RightHandSide[0];
            Assert.IsInstanceOfType(firstSymbol, typeof(StringLiteralLexerRule));

            var firstLexerRule = firstSymbol as StringLiteralLexerRule;
            Assert.AreEqual("a", firstLexerRule.Literal);

            var secondSymbol = production.RightHandSide[0];
            Assert.IsInstanceOfType(secondSymbol, typeof(StringLiteralLexerRule));

            var secondLexerRule = secondSymbol as StringLiteralLexerRule;
            Assert.AreEqual("a", secondLexerRule.Literal);
        }

        [TestMethod]
        public void Test_EbnfCompiler_Creates_Alteration_With_Two_Productions()
        {
            var grammar = Compile(@"Rule = 'a' | 'b';");

            Assert.AreEqual("Rule", grammar.Start.Value);
            Assert.AreEqual(2, grammar.Productions.Count);

            var firstProduction = grammar.Productions[0];
            Assert.AreEqual(firstProduction.LeftHandSide.Value, "Rule");
            Assert.AreEqual(firstProduction.RightHandSide.Count, 1);
            Assert.IsInstanceOfType(firstProduction.RightHandSide[0], typeof(StringLiteralLexerRule));
            var firstProductionFirstSymbol = firstProduction.RightHandSide[0] as StringLiteralLexerRule;
            Assert.AreEqual("a", firstProductionFirstSymbol.Literal);

            var secondProduction = grammar.Productions[1];
            Assert.AreEqual(secondProduction.LeftHandSide.Value, "Rule");
            Assert.AreEqual(secondProduction.RightHandSide.Count, 1);
            Assert.IsInstanceOfType(secondProduction.RightHandSide[0], typeof(StringLiteralLexerRule));
            var secondProductionSecondSymbol = secondProduction.RightHandSide[0] as StringLiteralLexerRule;
            Assert.AreEqual("b", secondProductionSecondSymbol.Literal);
        }

        [TestMethod]
        public void Test_EbnfCompiler_Creates_Alteration_With_Two_Productions_When_Given_Alteration_After_Contactenation()
        {
            var grammar = Compile(@"Rule = 'a' 'b' | 'c';");

            var firstProduction = grammar.Productions[0];
            Assert.AreEqual(firstProduction.LeftHandSide.Value, "Rule");
            Assert.AreEqual(firstProduction.RightHandSide.Count, 2);
            Assert.IsInstanceOfType(firstProduction.RightHandSide[0], typeof(StringLiteralLexerRule));
            var firstProductionFirstSymbol = firstProduction.RightHandSide[0] as StringLiteralLexerRule;
            Assert.AreEqual("a", firstProductionFirstSymbol.Literal);

            Assert.IsInstanceOfType(firstProduction.RightHandSide[0], typeof(StringLiteralLexerRule));
            var firstProductionSecondSymbol = firstProduction.RightHandSide[1] as StringLiteralLexerRule;
            Assert.AreEqual("b", firstProductionSecondSymbol.Literal);

            var secondProduction = grammar.Productions[1];
            Assert.AreEqual(secondProduction.LeftHandSide.Value, "Rule");
            Assert.AreEqual(secondProduction.RightHandSide.Count, 1);
            Assert.IsInstanceOfType(secondProduction.RightHandSide[0], typeof(StringLiteralLexerRule));
            var secondProductionSecondSymbol = secondProduction.RightHandSide[0] as StringLiteralLexerRule;
            Assert.AreEqual("c", secondProductionSecondSymbol.Literal);
        }

        [TestMethod]
        public void Test_EbnfCompiler_That_Given_Regex_Then_Creates_LexerRule()
        {
            var grammar = Compile(@"Rule = /[a-z]/;");
        }

        [TestMethod]
        public void Test_EbnfCompiler_That_Given_Braces_Then_Creates_Repetition()
        {
            var grammar = Compile(@"Rule = { 'a' };");
        }
        
        [TestMethod]
        public void Test_EbnfCompiler_That_Given_Brackets_Then_Creates_Optional_()
        {
            var grammar = Compile(@"Rule = [ 'a' ];");
        }

        [TestMethod]
        public void Test_EbnfCompiler_That_Given_Paranthesis_Then_Creates_Grouping()
        {
            var grammar = Compile(@"Rule = ('a');");
        }

        private static IGrammar Compile(string input)
        {
            var ebnfCompiler = new EbnfCompiler();
            return ebnfCompiler.Compile(input);
        }
    }
}
