using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Builders;
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
            var whitespace = new GrammarLexerRule(
                "whitespace", 
                new GrammarBuilder("whitespace", p=>p
                    .Production("whitespace", r=>r
                        .Rule(new WhitespaceTerminal(), "whitespace")
                        .Rule(new WhitespaceTerminal())))
                .GetGrammar());

            var grammarBuilder = new GrammarBuilder("syntax", g=>g
                .Production("syntax", r=>r
                    .Rule("syntax", "rule")
                    .Lambda())
                .Production("rule", r=>r
                    .Rule("identifier", '-', '>', "expression"))
                .Production("expression", r=>r
                    .Rule("term")
                    .Rule("term", "expression", '|', "term"))
                .Production("term", r=>r
                    .Rule("factor")
                    .Rule("factor", "term", "factor"))
                .Production("factor", r=>r
                    .Rule("identifier")
                    .Rule('(', "expression", ')')
                    .Rule('[', "expression", ']')
                    .Rule('{', "expression", '}'))
                .Production("identifier", r=>r
                    .Rule("letter")
                    .Rule("letter", "identifier", "letterOrDigit"))
                .Production("letterOrDigit", r=>r
                    .Rule("letter")
                    .Rule("digit"))
                .Production("letter", r=>r
                    .Rule(new RangeTerminal('a', 'z'))
                    .Rule(new RangeTerminal('A', 'Z')))
                .Production("digit", r=>r
                    .Rule(new DigitTerminal())), l=>l
                .LexerRule(whitespace), i=>i
                .Ignore("whitespace"));

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
            var parseEngine = new ParseEngine(grammar);
            var parseInterface = new ParseInterface(parseEngine, sampleBnf);
            var stringReader = new StringReader(sampleBnf);

            while (parseInterface.Read())
            {
                Assert.IsFalse(parseInterface.EndOfStream());
            }
            Assert.IsTrue(parseInterface.ParseEngine.IsAccepted(), 
                string.Format(
                    "Error parsing input string at position {0}", 
                    parseInterface.Position));
        }
    }
}
