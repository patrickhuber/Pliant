using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Builders;
using Pliant.Grammars;
using Pliant.RegularExpressions;
using System.Linq;

namespace Pliant.Tests.Unit
{
    [TestClass]
    public class GrammarBuilderTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void GrammarBuilderGivenProductionWithNoRightHandSideShouldAddEmptyProductionToList()
        {
            ProductionBuilder A = "A";
            var grammar = new GrammarBuilder(A, new[] { A }).ToGrammar();
            Assert.IsNotNull(grammar);
            Assert.AreEqual(1, grammar.Productions.Count);

            var production = grammar.Productions[0];
            Assert.IsNotNull(production);

            Assert.AreEqual(0, production.RightHandSide.Count);
        }

        [TestMethod]
        public void GrammarBuilderGivenProductionWithCharacterRightHandSideShouldAddTerminal()
        {
            ProductionBuilder A = "A";
            A.Definition = 'a';
            var grammar = new GrammarBuilder(A, new[] { A }).ToGrammar();
            Assert.IsNotNull(grammar);
            Assert.AreEqual(1, grammar.Productions.Count);

            var production = grammar.Productions[0];
            Assert.AreEqual(1, production.RightHandSide.Count);

            var symbol = production.RightHandSide[0];
            Assert.IsNotNull(symbol);
            Assert.AreEqual(SymbolType.LexerRule, symbol.SymbolType);
        }

        [TestMethod]
        public void GrammarBuilderGivenProductionWithStringRightHandSideShouldAddNonTerminal()
        {
            ProductionBuilder A = "A", B = "B";
            A.Definition = B;

            var grammarBuilder = new GrammarBuilder(A, new[] { A, B });

            var grammar = grammarBuilder.ToGrammar();
            Assert.IsNotNull(grammar);
            Assert.AreEqual(2, grammar.Productions.Count);

            var production = grammar.Productions[0];
            Assert.AreEqual(1, production.RightHandSide.Count);

            var symbol = production.RightHandSide[0];
            Assert.IsNotNull(symbol);
            Assert.AreEqual(SymbolType.NonTerminal, symbol.SymbolType);
        }

        [TestMethod]
        public void GrammarBuilderGivenProductionWithTwoCallsToRuleBuilderShouldCreateTwoProductions()
        {
            ProductionBuilder A = "A", B = "B", C = "C";
            A.Definition = B | C;
            var grammarBuilder = new GrammarBuilder(A, new[] { A, B, C });
            var grammar = grammarBuilder.ToGrammar();
            Assert.AreEqual(4, grammar.Productions.Count);
        }

        [TestMethod]
        public void GrammarBuilderGivenOnlyStartProductionBuilderShouldDiscoverLinkedProductions()
        {
            ProductionBuilder S = "S", A = "A", B = "B", C = "C";
            S.Definition = A | B;
            A.Definition = 'a';
            B.Definition = C;
            C.Definition = 'c';

            var grammarBuilder = new GrammarBuilder(S);
            var grammar = grammarBuilder.ToGrammar();
            Assert.AreEqual(5, grammar.Productions.Count);
        }

        [TestMethod]
        public void GrammarBuilderGivenOnlyStartProductionBuilderShouldShouldTraverseRecursiveStructuresOnlyOnce()
        {
            ProductionBuilder S = "S", A = "A";
            S.Definition = S | A;
            A.Definition = 'a';

            var grammarBuilder = new GrammarBuilder(S);
            var grammar = grammarBuilder.ToGrammar();
            Assert.AreEqual(3, grammar.Productions.Count);
        }

        [TestMethod]
        public void GrammarBuilderShouldResolveProductionReferencesFromOtherGrammars()
        {
            // namespace1.S -> namespace1.A
            // namespace1.A -> 'a'
            NamespaceBuilder ns1 = "namespace1";
            ProductionBuilder 
                S = ns1+ "S", 
                A = ns1 + "A";

            S.Definition = A;
            A.Definition = 'a';

            var ns1Grammar = new GrammarBuilder(S).ToGrammar();
            var ns1ProductionReference = new ProductionReference(ns1Grammar);

            // namespace2.Z -> namesapce2.X namespace1.S
            NamespaceBuilder ns2 = "namespace2";
            ProductionBuilder Z = ns2 + "Z", X = ns2 + "X";
            Z.Definition = X + ns1ProductionReference;

            var ns2Grammar = new GrammarBuilder(Z).ToGrammar();
            Assert.AreEqual(4, ns2Grammar.Productions.Count);
        }

        [TestMethod]
        public void GrammarBuilderShouldAddProductionWhenEmptyDefinition()
        {
            ProductionBuilder S = "S";
            var grammarBuilder = new GrammarBuilder(S);
            var grammar = grammarBuilder.ToGrammar();
            Assert.AreEqual(1, grammar.Productions.Count);
        }

        [TestMethod]
        public void GrammarBuilderGivenReferenceGrammarShouldGenerateWorkingGrammar()
        {
            var regexGrammar = new RegexGrammar();
            var regexProductionReference = new ProductionReference(regexGrammar);
            ProductionBuilder S = "S";
            S.Definition = regexProductionReference;
            var grammarBuilder = new GrammarBuilder();
            grammarBuilder.AddProduction(S);
            var grammar = grammarBuilder.ToGrammar();
            Assert.IsNotNull(grammar);
            Assert.AreEqual(1, grammar.Productions.Count);
        }

        [TestMethod]
        public void GrammarBuilderGivenNullStartShouldDetectStartFromProductions()
        {
            ProductionBuilder S = "S", A = "A", B = "B";
            S.Definition = A + B;
            A.Definition = 'a' + B;
            B.Definition = 'b';
            var grammarBuilder = new GrammarBuilder();
            grammarBuilder.AddProduction(S);
            grammarBuilder.AddProduction(A);
            grammarBuilder.AddProduction(B);
            var grammar = grammarBuilder.ToGrammar();
            Assert.AreEqual(3, grammar.Productions.Count);
            Assert.IsNotNull(grammar.Start);
        }
    }
}