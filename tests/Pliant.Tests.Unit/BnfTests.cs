using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Grammars;
using System.IO;

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
        public void Test_Bnf_That_Parse_Produces_Bnf_Chart()
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
             *  A -> B { B }
             *  equals
             *  A -> B A | B
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
            var grammarBuilder = new GrammarBuilder("syntax", g=>g
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
                    .Rule("quoted")
                    .Rule('(', "expression", ')')
                    .Rule('[', "expression", ']')
                    .Rule('{', "expression", '}'))
                .Production("identifier", p=>p
                    .Rule("letter")
                    .Rule("letter", "identifier", "letterOrDigit"))
                .Production("letterOrDigit", p=>p
                    .Rule("letter")
                    .Rule("digit"))
                .Production("letter", p=>p
                    .Rule(new RangeTerminal('a', 'z'))
                    .Rule(new RangeTerminal('A', 'Z')))
                .Production("digit", p=>p
                    .Rule(new DigitTerminal()))
                .Production("whitespace", p=>p
                    .Rule(new WhitespaceTerminal())));

            var grammar = grammarBuilder.GetGrammar();
            Assert.IsNotNull(grammar);

            var sampleBnf = @"
            syntax      ->  { rule }
            rule        ->  identifier '->' expression
            expression  ->  term { '|' }
            term        ->  factor { factor }
            factor      ->  identifier | 
                            quoted | 
                            '(' expression ')'
                            '[' expression ']'
                            '{' expression '}'
            identifier  ->  letter { letter | digit }
            quoted      ->  '""' { any } '""'
            letter      ->  '[a-zA-Z]'
            digit       ->  '[\d]'
            any         ->  '.'";
            var parseRunner = new ParseRunner(grammar, sampleBnf);
            var recognizer = new Recognizer(grammar);
            var stringReader = new StringReader(sampleBnf);
            Assert.IsTrue(recognizer.Recognize(stringReader));
        }
    }
}
