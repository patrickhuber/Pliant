using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Pliant.Tests.Unit
{
    /// <summary>
    ///     <see cref="http://cui.unige.ch/db-research/Enseignement/analyseinfo/AboutBNF.html"/>
    /// </summary>
    [TestClass]
    public class BnfTests
    {
        public TestContext TestContext { get; set; }

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
            var grammarBuilder = new GrammarBuilder(g=>g
                .Production("syntax", p=>p
                    .Rule("syntax", "rule")
                    .Lambda())
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
                    .Rule("digit"))
                .CharacterClass("letter", l=>l
                    .Range('a', 'z')
                    .Range('A', 'Z'))
                .CharacterClass("digit", l => l.Digit())
                .CharacterClass("whitespace", l=> l.WhiteSpace()));

            var grammar = grammarBuilder.GetGrammar();
            Assert.IsNotNull(grammar);
        }
    }
}
