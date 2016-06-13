using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Builders.Models;
using Pliant.Grammars;

namespace Pliant.Tests.Unit.Builders.Models
{
    [TestClass]
    public class GrammarModelTests
    {
        [TestMethod]
        public void GrammarModelShouldAddProductionModel()
        {
            var grammar = new GrammarModel();
            grammar.Productions.Add(
                new ProductionModel(""));
            Assert.AreEqual(1, grammar.Productions.Count);
        }

        [TestMethod]
        public void GrammarModelShouldAddIgnoreLexerRuleModel()
        {
            var grammar = new GrammarModel();
            grammar.IgnoreRules.Add(new LexerRuleModel {  Value = new StringLiteralLexerRule("this is a literal")});
            Assert.AreEqual(1, grammar.IgnoreRules.Count);
        }

        [TestMethod]
        public void GrammarModelToGrammarShouldCreateGrammar()
        {
            var grammarModel = new GrammarModel();

            var S = new ProductionModel("S");
            var A = new ProductionModel("A");
            var B = new ProductionModel("B");

            var a = new StringLiteralLexerRule("a");
            var b = new StringLiteralLexerRule("b");
            var space = new StringLiteralLexerRule(" ");

            S.AddWithAnd(A.LeftHandSide);
            S.AddWithAnd(B.LeftHandSide);
            S.AddWithOr(B.LeftHandSide);
            A.AddWithAnd(new LexerRuleModel(a));
            B.AddWithAnd(new LexerRuleModel(b));

            grammarModel.Productions.Add(S);
            grammarModel.Productions.Add(A);
            grammarModel.Productions.Add(B);

            grammarModel.IgnoreRules.Add(new LexerRuleModel(space));

            grammarModel.Start = S;

            var grammar = grammarModel.ToGrammar();
            Assert.AreEqual(4, grammar.Productions.Count);            
            Assert.AreEqual(1, grammar.Ignores.Count);
        }
    }
}
