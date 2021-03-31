using Pliant.Automata;
using Pliant.Grammars;

namespace Pliant.Languages.Bnf
{

    public class BnfGrammar : GrammarWrapper
    {
        private static readonly IGrammar _bnfGrammar;

        public static readonly string Namespace = "bnf";
        public static readonly FullyQualifiedName Grammar = new FullyQualifiedName(Namespace, nameof(Grammar));
        public static readonly FullyQualifiedName Rule = new FullyQualifiedName(Namespace, nameof(Rule));
        public static readonly FullyQualifiedName Expression = new FullyQualifiedName(Namespace, nameof(Expression));


        /// <summary>
        /// <code>
        /// grammar = 
        ///     rule | rule syntax ;
        /// rule = 
        ///     "&lt;" rule-name "&gt;" "::=" expresion line-end ;
        /// expression = 
        ///     list | list "|" expression;
        /// line-end   = 
        ///     EOL | line-end line-end;
        /// list       = 
        ///     term | term list;
        /// literal = 
        ///     single_quote_string | double_quote_string ;
        /// single_quote_string ~ /['][^']*[']/;
        /// double_quote_string ~ /["][^"]*["]/;
        /// </code>
        /// </summary>
        static BnfGrammar()
        {

            var whitespace = Whitespace();
            var ruleName = RuleName();
            var implements = Implements();
            var eol = EndOfLine();
            var doubleQuoteString = DoubleQuoteString();
            var singleQuoteString = SingleQuoteString();

            var grammar = new NonTerminal("grammar");
            var rule = new NonTerminal("rule");
            var identifier = new NonTerminal("identifier");
            var expression = new NonTerminal("expression");
            var lineEnd = new NonTerminal("line-end");
            var list = new NonTerminal("list");
            var term = new NonTerminal("term");
            var literal = new NonTerminal("literal");

            var lessThan = new TerminalLexerRule('<');
            var greaterThan = new TerminalLexerRule('>');
            var pipe = new TerminalLexerRule('|');

            var productions = new[]
            {
                new Production(grammar, rule),
                new Production(grammar, rule, grammar),
                new Production(rule, identifier, implements, expression),
                new Production(expression, list),
                new Production(expression, list, pipe, expression),
                new Production(lineEnd, eol),
                new Production(lineEnd, lineEnd, lineEnd),
                new Production(list, term),
                new Production(list, term, list),
                new Production(term, literal),
                new Production(term, identifier),
                new Production(identifier, lessThan, ruleName, greaterThan),
                new Production(literal, doubleQuoteString),
                new Production(literal, singleQuoteString),
            };

            var ignore = new[]
            {
                whitespace
            };

            _bnfGrammar = new Grammar(grammar, productions, ignore, null);
        }

        private static ILexerRule EndOfLine()
        {
            return new StringLiteralLexerRule("\r\n", new TokenType("eol"));
        }

        private static ILexerRule Implements()
        {
            return new StringLiteralLexerRule("::=", new TokenType("implements"));
        }

        private static ILexerRule RuleName()
        {
            var ruleNameState = new DfaState();
            var zeroOrMoreLetterOrDigit = new DfaState(true);
            ruleNameState.AddTransition(
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
            var ruleName = new DfaLexerRule(ruleNameState, new TokenType("rule-name"));
            return ruleName;
        }

        private static BaseLexerRule Whitespace()
        {
            var whitespaceTerminal = new WhitespaceTerminal();
            var startState = new DfaState();
            var finalState = new DfaState(true);
            var whitespaceTransition = new DfaTransition(whitespaceTerminal, finalState);
            startState.AddTransition(whitespaceTransition);
            finalState.AddTransition(whitespaceTransition);
            return new DfaLexerRule(startState, new TokenType("[\\s]+"));
        }

        private static ILexerRule SingleQuoteString()
        {
            var start = new DfaState();
            var loop = new DfaState();
            var esc = new DfaState();
            var end = new DfaState(isFinal: true);

            var singleQuote = new CharacterTerminal('\'');
            var notSingleQuote = new NegationTerminal(singleQuote);
            var escape = new CharacterTerminal('\\');
            var any = new AnyTerminal();

            // (start) - '   -> (loop)
            start.AddTransition(singleQuote, loop);

            // (loop) - [^'] -> (loop)
            //        - \    -> (esc)
            //        - '    -> (end)
            loop.AddTransition(notSingleQuote, loop);
            loop.AddTransition(escape, esc);
            loop.AddTransition(singleQuote, end);

            // (esc)  - .    -> (loop)
            esc.AddTransition(any, loop);

            return new DfaLexerRule(start, "single-quote-string");
        }


        private static ILexerRule DoubleQuoteString()
        {
            var start = new DfaState();
            var loop = new DfaState();
            var esc = new DfaState();
            var end = new DfaState(isFinal: true);

            var doubleQuote = new CharacterTerminal('"');
            var notDoubleQuote = new NegationTerminal(doubleQuote);
            var escape = new CharacterTerminal('\\');
            var any = new AnyTerminal();

            // (start) - "   -> (loop)
            start.AddTransition(doubleQuote, loop);

            // (loop) - [^"] -> (loop)
            //        - \    -> (esc)
            //        - "    -> (end)
            loop.AddTransition(notDoubleQuote, loop);
            loop.AddTransition(escape, esc);
            loop.AddTransition(doubleQuote, end);

            // (esc)  - .    -> (loop)
            esc.AddTransition(any, loop);

            return new DfaLexerRule(start, "double-quote-string");
        }

        public BnfGrammar()
            : base(_bnfGrammar)
        { }
    }
}
