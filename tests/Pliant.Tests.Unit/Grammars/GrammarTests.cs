using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Builders.Expressions;
using Pliant.LexerRules;
using Pliant.Languages.Regex;
using System.Linq;
using Pliant.Tests.Common.Grammars;

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

            foreach (var productionExpression in expectedEmpty)
            {
                var leftHandSide = productionExpression.ProductionModel.LeftHandSide.NonTerminal;
                Assert.IsTrue(grammar.IsNullable(leftHandSide), $"Expected {leftHandSide} to be transitive nullable");
            }
            foreach (var productionExpression in expectedNotEmpty)
            {
                var leftHandSide = productionExpression.ProductionModel.LeftHandSide.NonTerminal;
                Assert.IsFalse(grammar.IsNullable(leftHandSide), $"Expected {leftHandSide} to NOT be transitive nullable");
            }
        }

        [TestMethod]
        public void GrammarRulesContainingSymbolShouldReturnAllRulesContainingSymbol()
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

        [TestMethod]
        public void GrammarShouldContainAllLexerRulesInSuppliedProductionsIgnoresAndTrivia()
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

            var grammarExpression = new GrammarExpression(
                S,
                null,
                new[] { new WhitespaceLexerRule() },
                new[] { new WordLexerRule() });

            var grammar = grammarExpression.ToGrammar();

            Assert.IsNotNull(grammar.LexerRules);
            Assert.AreEqual(5, grammar.LexerRules.Count);
            for (var i = 0; i < grammar.LexerRules.Count; i++)
                Assert.IsNotNull(grammar.LexerRules[i]);
        }

        [TestMethod]
        public void GrammarShouldContainAllLexerRulesInReferencedGrammars()
        {
            var regex = new ProductionReferenceExpression(new RegexGrammar());
            ProductionExpression S = nameof(S);
            S.Rule = regex;
            var grammarExpression = new GrammarExpression(S);
            var grammar = grammarExpression.ToGrammar();
            Assert.IsNotNull(grammar.LexerRules);
            Assert.AreEqual(15, grammar.LexerRules.Count);
        }



        [TestMethod]
        public void GrammarShouldNotShowRecursionInNonRecursiveGrammar()
        {
            var grammar = new NullableGrammar();
            for (var p = 0; p < grammar.Productions.Count; p++)
            {
                var production = grammar.Productions[p];
                var symbol = production.LeftHandSide;
                Assert.IsFalse(grammar.IsRightRecursive(symbol), $"symbol {symbol} should not be recursive");
            }
        }

        [TestMethod]
        public void GrammarShouldDetectNullable()
        {
            var grammar = new NullableGrammar();
            for (var p = 0; p < grammar.Productions.Count; p++)
            {
                var production = grammar.Productions[p];
                Assert.IsTrue(grammar.IsNullable(production.LeftHandSide), $"symbol {production.LeftHandSide} should be nullable");
            }
        }

        [TestMethod]
        public void GrammarShouldDetectIndirectRightRecusion()
        {
            ProductionExpression
                S = nameof(S),
                A = nameof(A),
                B = nameof(B);

            S.Rule = A | S + B;
            B.Rule = S | 'b';
            A.Rule = 'a';

            var grammarExpression = new GrammarExpression(S);
            var grammar = grammarExpression.ToGrammar();
            var expectedRightRecursive = new[] { S, B };
            var expectedNotRightRecursive = new[] { A };

            foreach (var productionExpression in expectedRightRecursive)
            {
                var leftHandSide = productionExpression.ProductionModel.LeftHandSide.NonTerminal;
                Assert.IsTrue(grammar.IsRightRecursive(leftHandSide), $"Expected {leftHandSide} to be right recursive");
            }

            foreach (var productionExpression in expectedNotRightRecursive)
            {
                var leftHandSide = productionExpression.ProductionModel.LeftHandSide.NonTerminal;
                Assert.IsFalse(grammar.IsRightRecursive(leftHandSide), $"Expected {leftHandSide} to not be right recursive");
            }
        }

        [TestMethod]
        public void GrammarShouldDetectDirectRightRecursion()
        {
            ProductionExpression
                S = nameof(S),
                A = nameof(A);

            S.Rule = A | S + S;
            A.Rule = 'a';

            var grammarExpression = new GrammarExpression(S);
            var grammar = grammarExpression.ToGrammar();
            var expectedRightRecursive = new[] { S };
            var expectedNotRightRecursive = new[] { A };

            foreach (var productionExpression in expectedRightRecursive)
            {
                var leftHandSide = productionExpression.ProductionModel.LeftHandSide.NonTerminal;
                Assert.IsTrue(grammar.IsRightRecursive(leftHandSide), $"Expected {leftHandSide} to be right recursive");
            }

            foreach (var productionExpression in expectedNotRightRecursive)
            {
                var leftHandSide = productionExpression.ProductionModel.LeftHandSide.NonTerminal;
                Assert.IsFalse(grammar.IsRightRecursive(leftHandSide), $"Expected {leftHandSide} to not be right recursive");
            }
        }

        [TestMethod]
        public void GrammarShouldDetectNullableRightRecursion()
        {
            ProductionExpression
                S = nameof(S),
                A = nameof(A),
                B = nameof(B);

            S.Rule = A | S + B;
            A.Rule = 'a';
            B.Rule = 'b' | (Expr)null;

            var grammarExpression = new GrammarExpression(S);
            var grammar = grammarExpression.ToGrammar();
            var expectedRightRecursive = new[] { S };
            var expectedNotRightRecursive = new[] { A };

            foreach (var productionExpression in expectedRightRecursive)
            {
                var leftHandSide = productionExpression.ProductionModel.LeftHandSide.NonTerminal;
                Assert.IsTrue(grammar.IsRightRecursive(leftHandSide), $"Expected {leftHandSide} to be right recursive");
            }

            foreach (var productionExpression in expectedNotRightRecursive)
            {
                var leftHandSide = productionExpression.ProductionModel.LeftHandSide.NonTerminal;
                Assert.IsFalse(grammar.IsRightRecursive(leftHandSide), $"Expected {leftHandSide} to not be right recursive");
            }
        }

        [TestMethod]
        public void GrammarShouldNotShowRightRecursionInLeftRecursiveGrammar()
        {            
            ProductionExpression
                S = nameof(S),
                A = nameof(A),
                B = nameof(B);

            S.Rule = A | S + B;
            A.Rule = 'a';
            B.Rule = 'b' ;

            var grammarExpression = new GrammarExpression(S);
            var grammar = grammarExpression.ToGrammar();            
            var expectedNotRightRecursive = new[] { A,B,S };

            foreach (var productionExpression in expectedNotRightRecursive)
            {
                var leftHandSide = productionExpression.ProductionModel.LeftHandSide.NonTerminal;
                Assert.IsFalse(grammar.IsRightRecursive(leftHandSide), $"Expected {leftHandSide} to not be right recursive");
            }
        }
    }
}