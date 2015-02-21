using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Earley.Core.Tests
{
    /// <summary>
    ///     <see cref="http://cui.unige.ch/db-research/Enseignement/analyseinfo/AboutBNF.html"/>
    /// </summary>
    [TestClass]
    public class BnfTests
    {
        public BnfTests()
        {
           
        }

        public TestContext TestContext { get; set; }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void Test_Bnf_That_Parse_Produces_Bnf_Grammar()
        {
            /*  A -> B | C
             *  equals
             *  A -> B
             *  A -> C
             *  
             *  A -> { B }
             *  equals
             *  A -> A B | <null>
             *  
             *  A -> [ B ]
             *  equals
             *  A -> B | <null>
             *  
             *  Grammar
             *  -------
             *  syntax          ->  { rule }
             *  rule            ->  identifier '-' '>' expression
             *  expression      ->  term { '|' term }
             *  term            ->  factor { factor }
             *  factor          ->  identifier |
             *                      '(' expression ')' |
             *                      '[' expression ']' |
             *                      '{' expression '}'
             *  identifier      ->  letter { letter | digit }
             *  quoted_symbol   ->  '"' { any_character } '"'
             */
            var grammarBuilder = new GrammarBuilder()
                .Production("syntax", 
                    p => p.NonTerminal("syntax").NonTerminal("rule"),
                    null)
                .Production("syntax")
                .Production("rule", 
                    p => p.NonTerminal("identifier").Terminal('-').Terminal('>').NonTerminal("expression"))
                .Production("expression", 
                    p=>p.NonTerminal("term"),
                    p=>p.NonTerminal("term").NonTerminal("expression").Terminal('|').NonTerminal("term"))
                .Production("term", 
                    p=>p.NonTerminal("factor"),
                    p=>p.NonTerminal("term").NonTerminal("factor"))
                .Production("factor",
                    p=>p.NonTerminal("identifier"),
                    p=>p.Terminal('(').NonTerminal("expression").Terminal(')'));
            var grammar = grammarBuilder.GetGrammar();
        }
    }
}
