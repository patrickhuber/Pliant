using Pliant.Automata;
using Pliant.Builders.Expressions;
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
        public static readonly FullyQualifiedName Definition = new FullyQualifiedName(Namespace, nameof(Definition));
        public static readonly FullyQualifiedName Block = new FullyQualifiedName(Namespace, nameof(Block));
        public static readonly FullyQualifiedName Rule = new FullyQualifiedName(Namespace, nameof(Rule));
        public static readonly FullyQualifiedName Setting = new FullyQualifiedName(Namespace, nameof(Setting));
        public static readonly FullyQualifiedName LexerRule = new FullyQualifiedName(Namespace, nameof(LexerRule));
        public static readonly FullyQualifiedName QualifiedIdentifier = new FullyQualifiedName(Namespace, nameof(QualifiedIdentifier));
        public static readonly FullyQualifiedName Expression = new FullyQualifiedName(Namespace, nameof(Expression));
        public static readonly FullyQualifiedName Term = new FullyQualifiedName(Namespace, nameof(Term));
        public static readonly FullyQualifiedName Factor = new FullyQualifiedName(Namespace, nameof(Factor));
        public static readonly FullyQualifiedName Literal = new FullyQualifiedName(Namespace, nameof(Literal));
        public static readonly FullyQualifiedName Grouping = new FullyQualifiedName(Namespace, nameof(Grouping));
        public static readonly FullyQualifiedName Repetition = new FullyQualifiedName(Namespace, nameof(Repetition));
        public static readonly FullyQualifiedName Optional = new FullyQualifiedName(Namespace, nameof(Optional));
        public static readonly FullyQualifiedName LexerRuleExpression = new FullyQualifiedName(Namespace, nameof(LexerRuleExpression));
        public static readonly FullyQualifiedName LexerRuleTerm = new FullyQualifiedName(Namespace, nameof(LexerRuleTerm));
        public static readonly FullyQualifiedName LexerRuleFactor = new FullyQualifiedName(Namespace, nameof(LexerRuleFactor));

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

            ProductionExpression
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
                optional = Optional,
                lexerRuleExpression = LexerRuleExpression,
                lexerRuleTerm = LexerRuleTerm,
                lexerRuleFactor = LexerRuleFactor;

            var regexGrammar = new RegexGrammar();
            var regexProductionReference = new ProductionReferenceExpression(regexGrammar);
                        
            definition.Rule =
                block
                | block + definition;

            block.Rule =
                rule
                | setting
                | lexerRule;

            rule.Rule =
                qualifiedIdentifier + '=' + expression + ';';

            setting.Rule = (Expr)
                settingIdentifier + '=' + qualifiedIdentifier + ';';

            lexerRule.Rule =
                qualifiedIdentifier + '~' + lexerRuleExpression + ';';

            expression.Rule =
                term
                | term + '|' + expression;

            term.Rule =
                factor
                | factor + term;

            factor.Rule
                = qualifiedIdentifier
                | literal
                | '/' + regexProductionReference + '/'
                | repetition
                | optional
                | grouping;

            literal.Rule = (Expr)
                '"' + notDoubleQuote + '"'
                | (Expr)"'" + notSingleQuote + "'";

            repetition.Rule = (Expr)
                '{' + expression + '}';

            optional.Rule = (Expr)
                '[' + expression + ']';

            grouping.Rule = (Expr)
                '(' + expression + ')';

            qualifiedIdentifier.Rule =
                identifier
                | (Expr)identifier + '.' + qualifiedIdentifier;

            lexerRuleExpression.Rule = 
                lexerRuleTerm 
                | lexerRuleTerm + '|' + lexerRuleExpression;

            lexerRuleTerm.Rule =
                lexerRuleFactor
                | lexerRuleFactor + lexerRuleTerm;

            lexerRuleFactor.Rule =
                literal
                | '/' + regexProductionReference + '/';

            var grammarExpression = new GrammarExpression(
                definition, 
                new[] 
                {
                    definition,
                    block,
                    rule,
                    setting,
                    lexerRule,
                    expression,
                    term,
                    factor,
                    literal,
                    repetition,
                    optional,
                    grouping,
                    qualifiedIdentifier, 
                    lexerRuleExpression,
                    lexerRuleTerm,
                    lexerRuleFactor
                }, 
                new[] { new LexerRuleModel(whitespace) });
            _ebnfGrammar = grammarExpression.ToGrammar();
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

        public IReadOnlyList<IProduction> RulesFor(INonTerminal nonTerminal)
        {
            return _ebnfGrammar.RulesFor(nonTerminal);
        }

        public IEnumerable<IProduction> StartProductions()
        {
            return _ebnfGrammar.StartProductions();
        }

        public bool IsNullable(INonTerminal nonTerminal)
        {
            return _ebnfGrammar.IsNullable(nonTerminal);
        }
    }
}