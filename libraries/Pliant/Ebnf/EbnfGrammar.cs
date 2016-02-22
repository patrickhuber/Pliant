using Pliant.Automata;
using Pliant.Builders;
using Pliant.Grammars;
using Pliant.RegularExpressions;
using Pliant.Tokens;
using System.Collections.Generic;

namespace Pliant.Ebnf
{
    public class EbnfGrammar : IGrammar
    {
        private static readonly IGrammar _ebnfGrammar;

        public static readonly string Namespace = "Ebnf";
        public static readonly FullyQualifiedName Definition = new FullyQualifiedName(Namespace, "Definition");
        public static readonly FullyQualifiedName Block = new FullyQualifiedName(Namespace, "Block");
        public static readonly FullyQualifiedName Rule = new FullyQualifiedName(Namespace, "Rule");
        public static readonly FullyQualifiedName Setting = new FullyQualifiedName(Namespace, "Setting");
        public static readonly FullyQualifiedName LexerRule = new FullyQualifiedName(Namespace, "LexerRule");
        public static readonly FullyQualifiedName QualifiedIdentifier = new FullyQualifiedName(Namespace, "QualifiedIdentifier");
        public static readonly FullyQualifiedName Expression = new FullyQualifiedName(Namespace, "Expression");
        public static readonly FullyQualifiedName Term = new FullyQualifiedName(Namespace, "Term");
        public static readonly FullyQualifiedName Factor = new FullyQualifiedName(Namespace, "Factor");
        public static readonly FullyQualifiedName Literal = new FullyQualifiedName(Namespace, "Literal");
        public static readonly FullyQualifiedName Grouping = new FullyQualifiedName(Namespace, "Grouping");
        public static readonly FullyQualifiedName Repetition = new FullyQualifiedName(Namespace, "Repetition");
        public static readonly FullyQualifiedName Optional = new FullyQualifiedName(Namespace, "Optional");

        static EbnfGrammar()
        {
            BaseLexerRule
                settingIdentifier = CreateSettingIdentifierLexerRule(),
                notDoubleQuote = CreateNotDoubleQuoteLexerRule(),
                notSingleQuote = CreateNotSingleQuoteLexerRule(),
                identifier = CreateIdentifierLexerRule(),
                any = new TerminalLexerRule(new AnyTerminal(), "."),
                notCloseBracket = new TerminalLexerRule(
                    new NegationTerminal(new CharacterTerminal(']')), "[^\\]]"),
                notMeta = CreateNotMetaLexerRule(),
                escapeCharacter = CreateEscapeCharacterLexerRule(),
                whitespace = CreateWhitespaceLexerRule();

            ProductionBuilder
                definition = Definition,
                block = Block,
                rule = Rule,
                setting = Setting,
                lexerRule = LexerRule,
                qualifiedIdentifier = QualifiedIdentifier,
                expression = Expression,
                term = Term,
                factor = Factor,
                literal = Literal,
                grouping = Grouping,
                repetition = Repetition,
                optional = Optional;

            var regexGrammar = new RegexGrammar();
            var regexProductionReference = new ProductionReference(regexGrammar);
                        
            definition.Definition =
                block
                | block + definition;

            block.Definition =
                rule
                | setting
                | lexerRule;

            rule.Definition =
                qualifiedIdentifier + '=' + expression + ';';

            setting.Definition = (_)
                settingIdentifier + '=' + qualifiedIdentifier + ';';

            lexerRule.Definition =
                qualifiedIdentifier + '~' + expression + ';';

            expression.Definition =
                term
                | term + '|' + expression;

            term.Definition =
                factor
                | factor + term;

            factor.Definition
                = qualifiedIdentifier
                | literal
                | '/' + regexProductionReference + '/'
                | repetition
                | optional
                | grouping;

            literal.Definition = (_)
                '"' + notDoubleQuote + '"'
                | (_)"'" + notSingleQuote + "'";

            repetition.Definition = (_)
                '{' + expression + '}';

            optional.Definition = (_)
                '[' + expression + ']';

            grouping.Definition = (_)
                '(' + expression + ')';

            qualifiedIdentifier.Definition =
                identifier
                | (_)identifier + '.' + qualifiedIdentifier;
            
            var grammarBuilder = new GrammarBuilder(
                definition,                
                ignore: new[] { whitespace });

            _ebnfGrammar = grammarBuilder.ToGrammar();
        }

        private static BaseLexerRule CreateEscapeCharacterLexerRule()
        {
            var start = new DfaState();
            var escape = new DfaState();
            var final = new DfaState(true);
            start.AddTransition(new DfaTransition(new CharacterTerminal('\\'), escape));
            escape.AddTransition(new DfaTransition(new AnyTerminal(), final));
            return new DfaLexerRule(start, "escape");
        }

        private static BaseLexerRule CreateNotSingleQuoteLexerRule()
        {
            var start = new DfaState();
            var final = new DfaState(true);
            var terminal = new NegationTerminal(new CharacterTerminal('\''));
            var edge = new DfaTransition(terminal, final);
            start.AddTransition(edge);
            final.AddTransition(edge);
            return new DfaLexerRule(start, new TokenType(@"([^']|(\\.))+"));
        }

        private static BaseLexerRule CreateNotDoubleQuoteLexerRule()
        {
            // ([^"]|(\\.))*
            var start = new DfaState();
            var escape = new DfaState();
            var final = new DfaState(true);

            var notDoubleQuoteTerminal = new NegationTerminal(
                new CharacterTerminal('"'));
            var escapeTerminal = new CharacterTerminal('\\');
            var anyTerminal = new AnyTerminal();

            var notDoubleQuoteEdge = new DfaTransition(notDoubleQuoteTerminal, final);
            start.AddTransition(notDoubleQuoteEdge);
            final.AddTransition(notDoubleQuoteEdge);

            var escapeEdge = new DfaTransition(escapeTerminal, escape);
            start.AddTransition(escapeEdge);
            final.AddTransition(escapeEdge);

            var anyEdge = new DfaTransition(anyTerminal, final);
            escape.AddTransition(anyEdge);

            return new DfaLexerRule(start, new TokenType(@"([^""]|(\\.))+"));
        }

        private static BaseLexerRule CreateSettingIdentifierLexerRule()
        {
            // /:[a-zA-Z][a-zA-Z0-9]*/
            var start = new DfaState();
            var oneLetter = new DfaState();
            var zeroOrMoreLetterOrDigit = new DfaState(true);
            start.AddTransition(
               new DfaTransition(
                   new CharacterTerminal(':'),
                   oneLetter));
            oneLetter.AddTransition(
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
                        new DigitTerminal()),
                    zeroOrMoreLetterOrDigit));
            return new DfaLexerRule(start, "settingIdentifier");
        }

        private static BaseLexerRule CreateIdentifierLexerRule()
        {
            // /[a-zA-Z][a-zA-Z0-9-_]*/
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
            var identifier = new DfaLexerRule(identifierState, "identifier");
            return identifier;
        }

        private static BaseLexerRule CreateWhitespaceLexerRule()
        {
            var whitespaceTerminal = new WhitespaceTerminal();
            var startWhitespace = new DfaState();
            var finalWhitespace = new DfaState(true);
            startWhitespace.AddTransition(new DfaTransition(whitespaceTerminal, finalWhitespace));
            finalWhitespace.AddTransition(new DfaTransition(whitespaceTerminal, finalWhitespace));
            var whitespace = new DfaLexerRule(startWhitespace, "whitespace");
            return whitespace;
        }

        private static BaseLexerRule CreateNotMetaLexerRule()
        {
            return new TerminalLexerRule(
                    new NegationTerminal(
                        new SetTerminal('.', '^', '$', '(', ')', '[', ']', '+', '*', '?', '\\')),
                    "notMeta");
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