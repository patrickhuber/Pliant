using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Pliant.Tests.Unit
{
    [TestClass]
    public class GrammarBuilderTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test_GrammarBuilder_That_Production_With_No_RHS_Adds_Empty_Production_To_List()
        {
            var grammarBuilder = new GrammarBuilder("A", p=>p
                .Production("A", r=>r.Lambda()));
            
            var grammar = grammarBuilder.GetGrammar();
            Assert.IsNotNull(grammar);
            Assert.AreEqual(1, grammar.Productions.Count);
            
            var production = grammar.Productions[0];
            Assert.IsNotNull(production);

            Assert.AreEqual(0, production.RightHandSide.Count);
        }

        [TestMethod]
        public void Test_GrammarBuilder_That_Production_With_Character_RHS_Adds_Terminal()
        {
            var grammarBuilder = new GrammarBuilder(
                "A", p => p
                .Production("A", r => r.Rule('a')));
            
            var grammar = grammarBuilder.GetGrammar();
            Assert.IsNotNull(grammar);
            Assert.AreEqual(1, grammar.Productions.Count);
            
            var production = grammar.Productions[0];
            Assert.AreEqual(1, production.RightHandSide.Count);

            var symbol = production.RightHandSide[0];
            Assert.IsNotNull(symbol);
            Assert.AreEqual(SymbolType.Terminal, symbol.SymbolType);
        }

        [TestMethod]
        public void Test_GrammarBuilder_That_Production_With_String_RHS_Adds_NonTerminal()
        {
            var grammarBuilder = new GrammarBuilder("A", p=>p
                .Production("A", r=>r.Rule("B")));

            var grammar = grammarBuilder.GetGrammar();
            Assert.IsNotNull(grammar);
            Assert.AreEqual(1, grammar.Productions.Count);

            var production = grammar.Productions[0];
            Assert.AreEqual(1, production.RightHandSide.Count);

            var symbol = production.RightHandSide[0];
            Assert.IsNotNull(symbol);
            Assert.AreEqual(SymbolType.NonTerminal, symbol.SymbolType);
        }

        [TestMethod]
        public void Test_GrammarBuilder_That_Production_With_Two_Calls_To_RuleBuilder_Rule_Method_Creates_Two_Productions()
        {
            var grammarBuilder = new GrammarBuilder("A", p=>p
                .Production("A", r=>r.Rule("B").Rule("C")));
            var grammar = grammarBuilder.GetGrammar();
            Assert.AreEqual(2, grammar.Productions.Count);
        }

        [TestMethod]
        public void Test_GrammarBuilder_That_Lexeme_With_Two_Calls_To_Range_Lexeme_Method_Creates_Two_Lexemes()
        {
            var grammarBuilder = new GrammarBuilder("A", p => p
                .Production("A", r => r.Rule("B").Rule("C")), l => l
                .Lexeme("M", r => r.Range('a', 'z').Range('A', 'Z')));
            var grammar = grammarBuilder.GetGrammar();
            Assert.AreEqual(2, grammar.Lexemes.Count);
        }
    }
}
