using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Builders.Expressions;
using System.Linq;

namespace Pliant.Tests.Unit
{
    [TestClass]
    public class GrammarTests
    {
        [TestMethod]
        public void GrammarRulesForWhenProductionMatchesShouldReturnRules()
        {
            ProductionExpression B = "B", A = "A", S = "S";
            S.Rule = A | B;
            A.Rule = 'a';
            B.Rule = 'b';
            var grammar = new GrammarExpression(S, new[] { S, A, B })
                .ToGrammar();
            var rules = grammar.RulesFor(A.ProductionModel.LeftHandSide.NonTerminal).ToList();
            Assert.AreEqual(1, rules.Count);
            Assert.AreEqual(A.ProductionModel.LeftHandSide.NonTerminal.Value, rules[0].LeftHandSide.Value);
        }

        [TestMethod]
        public void GrammarShouldDiscoverEmptyProductionsWithTracibility()
        {
            ProductionExpression 
                S = nameof(S), 
                A = nameof(A), 
                B = nameof(B), 
                C = nameof(C), 
                D = nameof(D), 
                E = nameof(E), 
                F = nameof(F), 
                G = nameof(G);
            S.Rule = A + B;
            A.Rule = C + 'a';
            C.Rule = E;
            B.Rule = D;
            D.Rule = E + F;
            E.Rule = (Expr)null;
            F.Rule = G;
            G.Rule = (Expr)null;
            var grammar = new GrammarExpression(S, new[] { S, A, B, C, D, E, F, G }).ToGrammar();
            var expectedEmpty = new[] { B, C, D, E, F, G };
            var expectedNotEmpty = new[] { S, A };

            foreach(var productionBuilder in expectedEmpty)
                Assert.IsTrue(grammar.IsNullable(productionBuilder.ProductionModel.LeftHandSide.NonTerminal));
            foreach (var productionBuilder in expectedNotEmpty)
                Assert.IsFalse(grammar.IsNullable(productionBuilder.ProductionModel.LeftHandSide.NonTerminal));
        }

        [TestMethod]
        public void GrammarRulesConainingSymbolShouldReturnAllRulesContainingSymbol()
        {
            ProductionExpression
                S = nameof(S),
                A = nameof(A),
                B = nameof(B),
                C = nameof(C);

            S.Rule = A | B;
            A.Rule = A | C | 'a';
            B.Rule = 'b' | B;
            C.Rule = 'c';

            var grammarExpression = new GrammarExpression(S);
            var grammar = grammarExpression.ToGrammar();

            var rulesContainingA = grammar.RulesContainingSymbol(A.ProductionModel.LeftHandSide.NonTerminal);
            Assert.AreEqual(2, rulesContainingA.Count);

            var rulesContainingB = grammar.RulesContainingSymbol(B.ProductionModel.LeftHandSide.NonTerminal);
            Assert.AreEqual(2, rulesContainingB.Count);

            var rulesContainingC = grammar.RulesContainingSymbol(C.ProductionModel.LeftHandSide.NonTerminal);
            Assert.AreEqual(1, rulesContainingC.Count);

            var rulesContainingS = grammar.RulesContainingSymbol(S.ProductionModel.LeftHandSide.NonTerminal);
            Assert.AreEqual(0, rulesContainingS.Count);
        }
    }
}