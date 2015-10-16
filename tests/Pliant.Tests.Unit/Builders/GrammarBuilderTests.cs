using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Builders;
using Pliant.Grammars;

namespace Pliant.Tests.Unit
{
    [TestClass]
    public class GrammarBuilderTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test_GrammarBuilder_That_Production_With_No_RHS_Adds_Empty_Production_To_List()
        {
            var grammarBuilder = new GrammarBuilder("A")
                .Production("A", r=>r.Lambda());
            
            var grammar = grammarBuilder.ToGrammar();
            Assert.IsNotNull(grammar);
            Assert.AreEqual(1, grammar.Productions.Count);
            
            var production = grammar.Productions[0];
            Assert.IsNotNull(production);

            Assert.AreEqual(0, production.RightHandSide.Count);
        }

        [TestMethod]
        public void Test_GrammarBuilder_That_Production_With_Character_RHS_Adds_Terminal()
        {
            var grammarBuilder = new GrammarBuilder("A")
                .Production("A", r => r.Rule('a'));
            
            var grammar = grammarBuilder.ToGrammar();
            Assert.IsNotNull(grammar);
            Assert.AreEqual(1, grammar.Productions.Count);
            
            var production = grammar.Productions[0];
            Assert.AreEqual(1, production.RightHandSide.Count);

            var symbol = production.RightHandSide[0];
            Assert.IsNotNull(symbol);
            Assert.AreEqual(SymbolType.LexerRule, symbol.SymbolType);
        }

        [TestMethod]
        public void Test_GrammarBuilder_That_Production_With_String_RHS_Adds_NonTerminal()
        {
            var A = new ProductionBuilder("A");
            var B = new ProductionBuilder("B");

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
        public void Test_GrammarBuilder_That_Production_With_Two_Calls_To_RuleBuilder_Rule_Method_Creates_Two_Productions()
        {
            var A = new ProductionBuilder("A");
            var B = new ProductionBuilder("B");
            var C = new ProductionBuilder("C");

            A.Definition = B | C;
            var grammarBuilder = new GrammarBuilder(A, new[] { A, B, C });
            var grammar = grammarBuilder.ToGrammar();
            Assert.AreEqual(4, grammar.Productions.Count);
        }
        
    }
}
