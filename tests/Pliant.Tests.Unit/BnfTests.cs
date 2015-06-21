using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Builders;
using Pliant.Grammars;
using Pliant.Tokens;
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
            var whitespaceTerminal = new WhitespaceTerminal();
            var whitespace = new GrammarLexerRule(
                "whitespace",
                new GrammarBuilder("whitespace")
                    .Production("whitespace", r => r
                        .Rule(whitespaceTerminal, "whitespace")
                        .Rule(whitespaceTerminal))
                    .ToGrammar());

            var ruleName = new GrammarLexerRule(
                "rule-name",
                new GrammarBuilder("rule-name")
                    .Production("rule-name", r => r
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
                        .Rule(new DigitTerminal()))
                    .ToGrammar());

            var implements = new StringLiteralLexerRule("::=", new TokenType("implements"));
            var eol = new StringLiteralLexerRule("\r\n", new TokenType("eol"));

            var grammarBuilder = new GrammarBuilder("syntax")
                .Production("syntax", r => r
                    .Rule("syntax")
                    .Rule("rule", "syntax"))
                .Production("rule", r=>r
                    .Rule("identifier", implements, "expression", "line-end"))
                .Production("expression", r=>r
                    .Rule("list")
                    .Rule("list", '|', "expression"))
                .Production("line-end", r=>r
                    .Rule(eol, "line-end", "line-end"))
                .Production("list", r=>r
                    .Rule("term")
                    .Rule("term", "list"))
                .Production("term", r=>r
                    .Rule("literal")
                    .Rule("identifier"))
                .Production("identifier", r=>r
                    .Rule('<', ruleName, '>'))
                .Production("literal", r=>r
                    .Rule('"', "doubleQuoteText", '"')
                    .Rule('\'', "singleQuoteText", '\''))
                .Production("dobuleQuoteText", r=>r
                    .Rule("doubleQuoteText", new NegationTerminal(new Terminal('"')))
                    .Lambda())
                .Production("singleQuoteText", r=>r
                    .Rule("signleQuoteText", new NegationTerminal(new Terminal('\'')))
                    .Lambda())
                .Ignore(whitespace);
            var grammar = grammarBuilder.ToGrammar();
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
                if (!parseInterface.Read())
                {
                    var position = parseInterface.Position;
                    Assert.Fail(string.Format(
                        "Error parsing input string at position {0}. \r\n",
                        parseInterface.Position) + sampleBnf.Substring(position, 10));
                }
            }
        }
    }
}
