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
             *  <syntax>         ::= <rule> | <rule> <syntax>
             *  <rule>           ::= "<" <rule-name> ">" "::=" <expression> <line-end>
             *  <expression>     ::= <list> | <list> "|" <expression>
             *  <line-end>       ::= <EOL> | <line-end> <line-end>
             *  <list>           ::= <term> | <term> <list>
             *  <term>           ::= <literal> | "<" <rule-name> ">"
             *  <literal>        ::= '"' <text> '"' | "'" <text> 
             */
            var whitespaceTerminal = new SetTerminal(' ', '\t');
            var whitespace = new GrammarLexerRule(
                "whitespace",
                new GrammarBuilder("whitespace", p => p
                    .Production("whitespace", r => r
                        .Rule(whitespaceTerminal, "whitespace")
                        .Rule(whitespaceTerminal)))
                .GetGrammar());

            var identifier = new GrammarLexerRule(
                "identifier",
                new GrammarBuilder("identifier", p => p
                        .Production("identifier", r => r
                            .Rule("letter", "zeroOrManyLetterOrDigit"))
                        .Production("zeroOrManyLetterOrDigit", r => r
                            .Rule("letterOrDigit", "zeroOrManyLetterOrDigit")
                            .Lambda())
                        .Production("letterOrDigit", r => r
                            .Rule("letter")
                            .Rule("digit"))
                        .Production("letter", r => r
                            .Rule(new RangeTerminal('a', 'z'))
                            .Rule(new RangeTerminal('A', 'Z')))
                        .Production("digit", r => r
                            .Rule(new DigitTerminal())))
                    .GetGrammar());

            var grammarBuilder = new GrammarBuilder("syntax", p => p
                .Production("syntax", r => r
                    .Rule("syntax")
                    .Rule("rule", "syntax"))
                .Production("rule", r=>r
                    .Rule("identifier", ':', ':', '=', "expression", "line-end")));
            var grammar = grammarBuilder.GetGrammar();
            Assert.IsNotNull(grammar);

            var sampleBnf = @"
            <syntax>         ::= <rule> | <rule> <syntax>
            <rule>           ::= <identifier> ""::="" <expression> <line-end>
            <expression>     ::= <list> | <list> ""|"" <expression>
            <line-end>       ::= <EOL> | <line-end> <line-end>
            <list>           ::= <term > | <term> <list>
            <term>           ::= <literal > | <identifier>
            <identifier>     ::= ""<"" <rule-name> "">""
            <literal>        ::= '""' <text> '""' | ""'"" <text> ""'""";
            var parseEngine = new ParseEngine(grammar);
            var parseInterface = new ParseInterface(parseEngine, sampleBnf);
            var stringReader = new StringReader(sampleBnf);

            while (!parseInterface.EndOfStream())
            {
                Assert.IsTrue(
                    parseInterface.Read(),
                    string.Format(
                        "Error parsing input string at position {0}",
                        parseInterface.Position));
            }
            Assert.IsTrue(parseInterface.ParseEngine.IsAccepted(),
                "parser is not accepted");
        }
    }
}
