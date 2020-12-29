using Pliant.Automata;
using Pliant.Grammars;

namespace Pliant.Languages.Ebnf
{
    public class EbnfGrammar : GrammarWrapper
    {
        private static readonly IGrammar _ebnfGrammar;
        public static readonly string Namespace = "ebnf";

        public static readonly FullyQualifiedName Grammar = new FullyQualifiedName(Namespace, nameof(Grammar));
        public static readonly FullyQualifiedName Rule = new FullyQualifiedName(Namespace, nameof(Rule));
        public static readonly FullyQualifiedName LeftHandSide = new FullyQualifiedName(Namespace, nameof(LeftHandSide));        
        public static readonly FullyQualifiedName Repetition = new FullyQualifiedName(Namespace, nameof(Repetition));
        public static readonly FullyQualifiedName Optional = new FullyQualifiedName(Namespace, nameof(Optional));
        public static readonly FullyQualifiedName Group = new FullyQualifiedName(Namespace, nameof(Group));
        public static readonly FullyQualifiedName Expression = new FullyQualifiedName(Namespace, nameof(Expression));
        public static readonly FullyQualifiedName Concatenation = new FullyQualifiedName(Namespace, nameof(Concatenation));
        public static readonly FullyQualifiedName Alteration = new FullyQualifiedName(Namespace, nameof(Alteration));

        static EbnfGrammar() 
        {
            var grammar = new NonTerminal(Grammar);
            var rule = new NonTerminal(Rule);
            var leftHandSide = new NonTerminal(LeftHandSide);
            var expression = new NonTerminal(Expression);            
            var optional = new NonTerminal(Optional);
            var repetition = new NonTerminal(Repetition);
            var group = new NonTerminal(Group);
            var alteration = new NonTerminal(Alteration);
            var concatenation = new NonTerminal(Concatenation);

            var equal = new TerminalLexerRule('=');
            var semicolon = new TerminalLexerRule(';');
            var openBracket = new TerminalLexerRule('[');
            var closeBracket = new TerminalLexerRule(']');
            var openBrace = new TerminalLexerRule('{');
            var closeBrace = new TerminalLexerRule('}');
            var openParen = new TerminalLexerRule('(');
            var closeParen = new TerminalLexerRule(')');
            var pipe = new TerminalLexerRule('|');
            var comma = new TerminalLexerRule(',');

            var identifier = Identifier();
            var terminal = Terminal();

            var productions = new[]
            {
                new Production(grammar),
                new Production(grammar, grammar, rule),
                new Production(rule, leftHandSide, equal, expression, semicolon),
                new Production(leftHandSide, identifier),
                new Production(expression, identifier),
                new Production(expression, terminal),
                new Production(expression, optional),
                new Production(expression, repetition),
                new Production(expression, group),
                new Production(expression, alteration),
                new Production(expression, concatenation),
                new Production(optional, openBracket, expression, closeBracket),
                new Production(repetition, openBrace, expression, closeBrace),
                new Production(group, openParen, expression, closeParen),
                new Production(alteration, expression, pipe, expression),
                new Production(concatenation, expression, comma, expression)
            };

            var whitespace = Whitespace();

            var ignore = new[] 
            {
                whitespace
            };

            _ebnfGrammar = new Grammar(grammar, productions, ignore, null);
        }

        /// <summary>
        /// Creates an identifier lexer rule. Identifiers are defined as:
        /// <code>
        /// identifier ~ 
        ///     letter { letter | digit | "_" };
        /// </code>
        /// </summary>
        /// <returns></returns>
        private static BaseLexerRule Identifier()
        {
            var start = new DfaState();
            var end = new DfaState(isFinal: true);
            start.AddTransition(                
                new LetterTerminal(),
                end);
            end.AddTransition(
                new LetterTerminal(),
                end);
            end.AddTransition(
                new DigitTerminal(),
                end);
            end.AddTransition(
                new CharacterTerminal('_'),
                end);

            return new DfaLexerRule(start, "identifier");
        }


        /// <summary>
        /// Creates a terminal lexer rule. The grammar is defined as:
        /// <code>
        /// terminal ~
        ///    "'" character {character } "'" 
        ///    | '"' character {character } '"';
        /// </code>         
        /// </summary>
        /// <returns></returns>
        private static BaseLexerRule Terminal()
        {
            var start = new DfaState();
            var doubleQuoteString = new DfaState();
            var singleQuoteString = new DfaState();
            var end = new DfaState(isFinal: true);

            var doubleQuote = new CharacterTerminal('"');
            var singleQuote = new CharacterTerminal('\'');

            start.AddTransition(doubleQuote, doubleQuoteString);
            start.AddTransition(singleQuote, singleQuoteString);

            doubleQuoteString.AddTransition(new NegationTerminal(doubleQuote), doubleQuoteString);
            doubleQuoteString.AddTransition(doubleQuote, end);

            singleQuoteString.AddTransition(new NegationTerminal(singleQuote), singleQuoteString);
            singleQuoteString.AddTransition(singleQuote, end);

            return new DfaLexerRule(start, "terminal");
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

        public EbnfGrammar() : base(_ebnfGrammar)
        {
        }
    }
}
