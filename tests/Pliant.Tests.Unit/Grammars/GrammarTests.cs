using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Builders;
using Pliant.Grammars;
using System.Collections.Generic;
using System.Linq;

namespace Pliant.Tests.Unit
{
    [TestClass]
    public class GrammarTests
    {
        [TestMethod]
        public void GrammarRulesForWhenProductionMatchesShouldReturnRules()
        {
            ProductionBuilder B = "B", A = "A", S = "S";
            S.Definition = A | B;
            A.Definition = 'a';
            B.Definition = 'b';
            var grammar = new GrammarBuilder(S, new[] { S, A, B })
                .ToGrammar();
            var rules = grammar.RulesFor(A.LeftHandSide).ToList();
            Assert.AreEqual(1, rules.Count);
            Assert.AreEqual(A.LeftHandSide.Value, rules[0].LeftHandSide.Value);
        }

        [TestMethod]
        public void GrammarShouldDiscoverEmptyProductionsWithTracibility()
        {
            ProductionBuilder 
                S = nameof(S), 
                A = nameof(A), 
                B = nameof(B), 
                C = nameof(C), 
                D = nameof(D), 
                E = nameof(E), 
                F = nameof(F), 
                G = nameof(G);
            S.Definition = A + B;
            A.Definition = C + 'a';
            C.Definition = E;
            B.Definition = D;
            D.Definition = E + F;
            E.Definition = (_)null;
            F.Definition = G;
            G.Definition = (_)null;
            var grammar = new GrammarBuilder(S, new[] { S, A, B, C, D, E, F, G }).ToGrammar();
            var expectedEmpty = new[] { B, C, D, E, F, G };
            var expectedNotEmpty = new[] { S, A };

            foreach(var productionBuilder in expectedEmpty)
                Assert.IsTrue(grammar.IsNullable(productionBuilder.LeftHandSide));
            foreach (var productionBuilder in expectedNotEmpty)
                Assert.IsFalse(grammar.IsNullable(productionBuilder.LeftHandSide));
        }
    }
}