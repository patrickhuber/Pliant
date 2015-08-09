using Pliant.Automata;
using Pliant.Grammars;
using Pliant.Tokens;
using System.Collections.Generic;
using System;

namespace Pliant.Ebnf
{
    public class EbnfGrammar : IGrammar
    {
        private static readonly IGrammar _ebnfGrammar;

        static EbnfGrammar()
        {
            /* 
             *  Grammar        = { Rule } 
             *  Rule           = RuleName "=" Expression ";"
             *  Expression     = List { "|" List }
             *  List           = Term { Term }
             *  Term           = Literal | RuleName
             *  Literal        = '"' Text '"' | "'" Text "'"
             */
            var whitespace = CreateWhitespaceLexerRule();
            var notDoubleQuote = CreateNotDoubleQuoteLexerRule();
            var notSingleQuuote = CreateNotSingleQuoteLexerRule();
            var identifier = CreateIdentifierLexerRule();

            var grammar = new NonTerminal("Grammar");
            var rule = new NonTerminal("Rule");
            var expression = new NonTerminal("Expression");
            var term = new NonTerminal("Term");
            var literal = new NonTerminal("Literal");
            var doubleQuoteText = new NonTerminal("DoubleQuoteText");
            var singleQuoteText = new NonTerminal("SingleQuoteText");
            var factor = new NonTerminal("Factor");
            var atom = new NonTerminal("Atom");

            var productions = new[]
            {
                new Production(grammar, rule, grammar),
                new Production(grammar),
                new Production(rule, identifier, new TerminalLexerRule('='), expression, new TerminalLexerRule(';')),
                new Production(expression, identifier),
                new Production(expression, literal),
                new Production(literal, new TerminalLexerRule('"'), notDoubleQuote, new TerminalLexerRule('"')),
                new Production(literal, new TerminalLexerRule('\''), notSingleQuuote, new TerminalLexerRule('\''))
            };

            var ignore = new[]
            {
                whitespace
            };

            _ebnfGrammar = new Grammar(grammar, productions, ignore);
        }

        private static ILexerRule CreateNotSingleQuoteLexerRule()
        {
            var start = new DfaState();
            var final = new DfaState(true);
            var terminal = new NegationTerminal(new Terminal('\''));
            var edge = new DfaTransition(terminal, final);
            start.AddTransition(edge);
            final.AddTransition(edge);
            return new DfaLexerRule(start, new TokenType("not-single-quote"));
        }

        private static ILexerRule CreateNotDoubleQuoteLexerRule()
        {
            // ( [^"\\] | (\\ .) ) +
            var start = new DfaState();
            var escape = new DfaState();
            var final = new DfaState(true);

            var notQuoteTerminal = new NegationTerminal(
                new SetTerminal('"', '\\'));
            var escapeTerminal = new Terminal('\\');
            var anyTerminal = new AnyTerminal();

            var notQuoteEdge = new DfaTransition(notQuoteTerminal, final);
            start.AddTransition(notQuoteEdge);
            final.AddTransition(notQuoteEdge);

            var escapeEdge = new DfaTransition(escapeTerminal, escape);
            start.AddTransition(escapeEdge);
            final.AddTransition(escapeEdge);

            var anyEdge = new DfaTransition(anyTerminal, final);
            escape.AddTransition(anyEdge);

            return new DfaLexerRule(start, new TokenType("not-double-quote"));
        }
        
        private static ILexerRule CreateIdentifierLexerRule()
        {
            var identifierState = new DfaState();
            var zeroOrMoreLetterOrDigit = new DfaState(true);
            identifierState.AddTransition(
                new DfaTransition(
                    new CharacterClassTerminal(
                        new RangeTerminal('a', 'z'),
                        new RangeTerminal('A', 'Z')),
                    zeroOrMoreLetterOrDigit));
            zeroOrMoreLetterOrDigit.AddTransition(
                new DfaTransition(
                    new CharacterClassTerminal(
                        new RangeTerminal('a', 'z'),
                        new RangeTerminal('A', 'Z'),
                        new DigitTerminal(),
                        new SetTerminal('-', '_')),
                    zeroOrMoreLetterOrDigit));
            var identifier = new DfaLexerRule(identifierState, new TokenType("identifier"));
            return identifier;
        }

        private static ILexerRule CreateWhitespaceLexerRule()
        {
            var whitespaceTerminal = new WhitespaceTerminal();
            var startWhitespace = new DfaState();
            var finalWhitespace = new DfaState(true);
            startWhitespace.AddTransition(new DfaTransition(whitespaceTerminal, finalWhitespace));
            finalWhitespace.AddTransition(new DfaTransition(whitespaceTerminal, finalWhitespace));
            var whitespace = new DfaLexerRule(startWhitespace, new TokenType("whitespace"));
            return whitespace;
        }

        public IReadOnlyList<ILexerRule> Ignores
        {
            get { return _ebnfGrammar.Ignores; }
        }

        public IReadOnlyList<IProduction> Productions
        {
            get { return _ebnfGrammar.Productions; }
        }

        public INonTerminal Start
        {
            get { return _ebnfGrammar.Start; }
        }

        public IEnumerable<IProduction> RulesFor(INonTerminal nonTerminal)
        {
            return _ebnfGrammar.RulesFor(nonTerminal);
        }

        public IEnumerable<IProduction> StartProductions()
        {
            return _ebnfGrammar.StartProductions();
        }
    }
}
