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
            var input = @"Rule = 'a';";
            var ebnfCompiler = new EbnfCompiler();
            var grammar = ebnfCompiler.Compile(input);

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
            var input = @"Rule = 'a' 'b';";
            var ebnfCompiler = new EbnfCompiler();
            var grammar = ebnfCompiler.Compile(input);

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
            var input = @"Rule = 'a' | 'b';";
            var ebnfCompiler = new EbnfCompiler();
            var grammar = ebnfCompiler.Compile(input);

            Assert.AreEqual("Rule", grammar.Start.Value);
            Assert.AreEqual(2, grammar.Productions.Count);
        }
    }
}
