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
            var objarr = new object[] { "a", 'b' };
            var grammarBuilder = new GrammarBuilder();
            grammarBuilder
                .Production("syntax", p=>p
                    .Rule("syntax", "rule")
                    .Rule())
                .Production("rule", p=>p
                    .Rule("identifier", '-', '>', "expression"))
                .Production("expression", p=>p
                    .Rule("term")
                    .Rule("term", "expression", '|', "term"))
                .Production("term", p=>p
                    .Rule("factor")
                    .Rule("factor", "term", "factor"))
                .Production("factor", p=>p
                    .Rule("identifier")
                    .Rule('(', "identifier", ')')
                    .Rule('[', "identifier", ']')
                    .Rule('{', "identifier", '}'))
                .Production("identifier", p=>p
                    .Rule("letter")
                    .Rule("letter", "identifier", "letterOrDigit"))
                .Production("letterOrDigit", p=>p
                    .Rule("letter")
                    .Rule("digit"));
            for(char c = 'a';c<'z';c++)
                grammarBuilder.Production("letter", p=>p.Rule(c));
            for(char c = 'A';c<'Z';c++)
                grammarBuilder.Production("letter", p => p.Rule(c));
            for(char d = '0';d<'9';d++)
                grammarBuilder.Production("digit", p => p.Rule(d));

            var grammar = grammarBuilder.GetGrammar();
            Assert.IsNotNull(grammar);
        }
    }
}
