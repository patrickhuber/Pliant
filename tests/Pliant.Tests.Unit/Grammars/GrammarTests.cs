using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Builders.Expressions;
using Pliant.LexerRules;
using Pliant.Languages.Regex;
using System.Linq;
using Pliant.Tests.Common.Grammars;
using Pliant.Grammars;

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
                Assert.IsFalse(grammar.IsRightRecursive(production), $"production {production} should not be recursive");
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

            var rightRecursive = new (string production, int alteration)[] { (nameof(S), 1), (nameof(B), 0) };
            var notRightRecursive = new (string production, int alteration)[] { (nameof(S), 0), (nameof(B), 1), (nameof(A), 0) };

            AssertRightRecursion(grammar, rightRecursive, true);
            AssertRightRecursion(grammar, notRightRecursive, false);
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

            var rightRecursive = new (string production, int alteration)[] { (nameof(S), 1) };
            var notRightRecursive = new (string production, int alteration)[] { (nameof(S), 0), (nameof(A), 0) };

            AssertRightRecursion(grammar, rightRecursive, true);
            AssertRightRecursion(grammar, notRightRecursive, false);
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

            var rightRecursive = new (string production, int alteration)[] { (nameof(S), 1) };
            var notRightRecursive = new (string production, int alteration)[] { (nameof(S), 0), (nameof(B), 0), (nameof(A), 0) };

            AssertRightRecursion(grammar, rightRecursive, true);
            AssertRightRecursion(grammar, notRightRecursive, false);
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
            B.Rule = 'b';

            var grammarExpression = new GrammarExpression(S);
            var grammar = grammarExpression.ToGrammar();
            var expectedNotRightRecursive = new[] { (nameof(S), 0), (nameof(S), 1), (nameof(A), 0), (nameof(B), 0) };
            AssertRightRecursion(grammar, expectedNotRightRecursive, false);
        }



        [TestMethod]
        public void GrammarIsRightRecursiveShouldNotContainSymbolsWithoutCycles()
        {
            ProductionExpression
                A = nameof(A),
                B = nameof(B),
                C = nameof(C),
                D = nameof(D),
                E = nameof(E);

            A.Rule = B + C;
            B.Rule = 'b';
            C.Rule = A | D;
            D.Rule = E + D | 'd';
            E.Rule = 'e';

            var grammar = new GrammarExpression(A).ToGrammar();

            var rightRecursive = new (string production, int alteration)[] { (nameof(A), 0), (nameof(C), 0), (nameof(D), 0) };
            var notRightRecursive = new (string production, int alteration)[] { (nameof(B), 0), (nameof(E), 0), (nameof(D), 1), (nameof(C), 1) };

            AssertRightRecursion(grammar, rightRecursive, true);
            AssertRightRecursion(grammar, notRightRecursive, false);
        }

        private void AssertRightRecursion(IGrammar grammar, (string production, int alteration)[] tests, bool shouldBeRecursive)
        {            
            foreach (var tuple in tests)
            {
                var count = -1;
                var found = false;
                for (int i = 0; i < grammar.Productions.Count; i++)
                {
                    var production = grammar.Productions[i];
                    if (production.LeftHandSide.ToString() != tuple.production)
                        continue;
                    count++;

                    if (count != tuple.alteration)
                        continue;

                    if(shouldBeRecursive)
                        Assert.IsTrue(grammar.IsRightRecursive(production), $"expected {production} to be right recursive");
                    else
                        Assert.IsFalse(grammar.IsRightRecursive(production), $"expected {production} to not be right recursive");
                    found = true;
                    break;
                }
                Assert.IsTrue(found, $"the rule {tuple.production} {tuple.alteration} was not found");
            }
        }
    }
}