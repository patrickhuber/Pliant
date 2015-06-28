using Pliant.Builders;
using Pliant.Dfa;
using Pliant.Grammars;
using Pliant.Tokens;
using System.Collections.Generic;

namespace Pliant.Bnf
{
    public class BnfGrammar : IGrammar
    {
        private static readonly IGrammar _bnfGrammar;

        static BnfGrammar()
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
            var startWhitespace = new DfaState();
            var finalWhitespace = new DfaState(true);
            startWhitespace.AddEdge(new DfaEdge(whitespaceTerminal, finalWhitespace));
            finalWhitespace.AddEdge(new DfaEdge(whitespaceTerminal, finalWhitespace));
            var whitespace = new DfaLexerRule(startWhitespace, new TokenType("whitespace"));

            var ruleNameState = new DfaState();
            var zeroOrMoreLetterOrDigit = new DfaState(true);
            ruleNameState.AddEdge(
                new DfaEdge(
                    new CharacterClassTerminal(
                        new RangeTerminal('a', 'z'),
                        new RangeTerminal('A', 'Z')),
                    zeroOrMoreLetterOrDigit));
            zeroOrMoreLetterOrDigit.AddEdge(
                new DfaEdge(
                    new CharacterClassTerminal(
                        new RangeTerminal('a', 'z'),
                        new RangeTerminal('A', 'Z'),
                        new DigitTerminal(),
                        new SetTerminal('-', '_')),
                    zeroOrMoreLetterOrDigit));
            var ruleName = new DfaLexerRule(ruleNameState, new TokenType("rule-name"));

            var implements = new StringLiteralLexerRule("::=", new TokenType("implements"));
            var eol = new StringLiteralLexerRule("\r\n", new TokenType("eol"));
                        
            var grammarBuilder = new GrammarBuilder("syntax")
                .Production("syntax", r => r
                    .Rule("rule")
                    .Rule("rule", "syntax"))
                .Production("rule", r => r
                    .Rule("identifier", implements, "expression", "line-end"))
                .Production("expression", r => r
                    .Rule("list")
                    .Rule("list", '|', "expression"))
                .Production("line-end", r => r
                    .Rule(eol)
                    .Rule("line-end", "line-end"))
                .Production("list", r => r
                    .Rule("term")
                    .Rule("term", "list"))
                .Production("term", r => r
                    .Rule("literal")
                    .Rule("identifier"))
                .Production("identifier", r => r
                    .Rule('<', ruleName, '>'))
                .Production("literal", r => r
                    .Rule('"', "doubleQuoteText", '"')
                    .Rule('\'', "singleQuoteText", '\''))
                .Production("doubleQuoteText", r => r
                    .Rule("doubleQuoteText", new NegationTerminal(new Terminal('"')))
                    .Lambda())
                .Production("singleQuoteText", r => r
                    .Rule("singleQuoteText", new NegationTerminal(new Terminal('\'')))
                    .Lambda())
                .Ignore(whitespace);
            _bnfGrammar = grammarBuilder.ToGrammar();
        }

        public IReadOnlyList<IProduction> Productions
        {
            get { return _bnfGrammar.Productions; }
        }

        public INonTerminal Start
        {
            get { return _bnfGrammar.Start; }
        }

        public IReadOnlyList<ILexerRule> Ignores
        {
            get { return _bnfGrammar.Ignores; }
        }

        public IReadOnlyList<ILexerRule> LexerRules
        {
            get { return _bnfGrammar.LexerRules; }
        }

        public IEnumerable<IProduction> RulesFor(INonTerminal nonTerminal)
        {
            return _bnfGrammar.RulesFor(nonTerminal);
        }

        public IEnumerable<ILexerRule> LexerRulesFor(INonTerminal nonTerminal)
        {
            return _bnfGrammar.LexerRulesFor(nonTerminal);
        }

        public IEnumerable<IProduction> StartProductions()
        {
            return _bnfGrammar.StartProductions();
        }
    }
}
