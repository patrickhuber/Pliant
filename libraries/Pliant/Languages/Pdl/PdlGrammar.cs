﻿using Pliant.Automata;
using Pliant.Builders;
using Pliant.Builders.Expressions;
using Pliant.Grammars;
using Pliant.Languages.Regex;
using Pliant.LexerRules;

namespace Pliant.Languages.Pdl
{
    public class PdlGrammar : GrammarWrapper
    {
        private static readonly IGrammar _pdlGrammar;

        public static readonly string Namespace = "pdl";
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

        static PdlGrammar()
        {
            BaseLexerRule
                settingIdentifier = SettingIdentifier(),
                identifier = Identifier(),
                whitespace = Whitespace(),
                multiLineComment = MultiLineComment();

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
                new SingleQuoteStringLexerRule()
                | new DoubleQuoteStringLexerRule();

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
                new[] { new LexerRuleModel(whitespace), new LexerRuleModel(multiLineComment) });
            _pdlGrammar = grammarExpression.ToGrammar();
        }

        public PdlGrammar() : base(_pdlGrammar)
        {
        }

        public static class TokenTypes
        {
            public static readonly TokenType Escape = new TokenType("escape");
            public static readonly TokenType NotSingleQuote = new TokenType(@"([^']|(\\.))+");
            public static readonly TokenType NotDoubleQuote = new TokenType(@"([^""]|(\\.))+");
            public static readonly TokenType SettingIdentifier = new TokenType("settingIdentifier");
            public static readonly TokenType Identifier = new TokenType("identifier");
            public static readonly TokenType Whitespace = new TokenType("whitespace");
            public static readonly TokenType MultiLineComment = new TokenType(@"\/[*]([*][^\/]|[^*])*[*][\/]");
        }

        private static BaseLexerRule SettingIdentifier()
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
            return new DfaLexerRule(start, TokenTypes.SettingIdentifier);
        }

        private static BaseLexerRule Identifier()
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
            var identifier = new DfaLexerRule(identifierState, TokenTypes.Identifier);
            return identifier;
        }

        private static BaseLexerRule Whitespace()
        {
            var whitespaceTerminal = new WhitespaceTerminal();
            var startWhitespace = new DfaState();
            var finalWhitespace = new DfaState(true);
            startWhitespace.AddTransition(new DfaTransition(whitespaceTerminal, finalWhitespace));
            finalWhitespace.AddTransition(new DfaTransition(whitespaceTerminal, finalWhitespace));
            var whitespace = new DfaLexerRule(startWhitespace, TokenTypes.Whitespace);
            return whitespace;
        }

        private static BaseLexerRule MultiLineComment()
        {
            var states = new DfaState[5];
            for (int i = 0; i < states.Length; i++)
                states[i] = new DfaState();

            var slash = new CharacterTerminal('/');
            var star = new CharacterTerminal('*');
            var notStar = new NegationTerminal(star);
            var notSlash = new NegationTerminal(slash);

            var firstSlash = new DfaTransition(slash, states[1]);
            var firstStar = new DfaTransition(star, states[2]);
            var repeatNotStar = new DfaTransition(notStar, states[2]);
            var lastStar = new DfaTransition(star, states[3]);
            var goBackNotSlash = new DfaTransition(notSlash, states[2]);
            var lastSlash = new DfaTransition(slash, states[4]);

            states[0].AddTransition(firstSlash);
            states[1].AddTransition(firstStar);
            states[2].AddTransition(repeatNotStar);
            states[2].AddTransition(lastStar);
            states[3].AddTransition(goBackNotSlash);
            states[3].AddTransition(lastSlash);

            return new DfaLexerRule(states[0], TokenTypes.MultiLineComment);
        }
    }
}
