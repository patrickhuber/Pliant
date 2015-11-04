using Pliant.Automata;
using Pliant.Builders;
using Pliant.Grammars;
using Pliant.Tokens;
using System.Collections.Generic;

namespace Pliant.Ebnf
{
    public class EbnfGrammar : IGrammar
    {
        private static readonly IGrammar _ebnfGrammar;

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
                Grammar = "Grammar",
                Block = "Block",
                Rule = "Rule",
                Setting = "Setting",
                LexerRule = "LexerRule",
                QualifiedIdentifier = "QualifiedIdentifier",
                Expression = "Expression",
                Term = "Term",
                Factor = "Factor",
                Literal = "Literal",
                Grouping = "Grouping",
                Repetition = "Repetition",
                Optional = "Optional",
                Regex = "Regex",
                RegexExpression = "Regex.Expression",
                RegexTerm = "Regex.Term",
                RegexFactor = "Regex.Factor",
                RegexAtom = "Regex.Atom",
                RegexIterator = "Regex.Iterator",
                RegexSet = "Regex.Set",
                RegexPositiveSet = "Regex.PositiveSet",
                RegexNegativeSet = "Regex.NegativeSet",
                RegexCharacterClass = "Regex.CharacterClass",
                RegexCharacterRange = "Regex.CharacterRange",
                RegexCharacter = "Regex.Character",
                RegexCharacterClassCharacter = "Regex.CharacterClassCharacter";

            Grammar.Definition =
                Block
                | Block + Grammar;

            Block.Definition =
                Rule
                | Setting
                | LexerRule;

            Rule.Definition =
                QualifiedIdentifier + '=' + Expression + ';';

            Setting.Definition = (_)
                settingIdentifier + '=' + QualifiedIdentifier + ';';

            LexerRule.Definition =
                QualifiedIdentifier + '~' + Expression + ';';

            Expression.Definition =
                Term
                | Term + '|' + Expression;

            Term.Definition =
                Factor
                | Factor + Term;

            Factor.Definition
                = QualifiedIdentifier
                | Literal
                | '/' + Regex + '/'
                | Repetition
                | Optional
                | Grouping;

            Literal.Definition = (_)
                '"' + notDoubleQuote + '"'
                | (_)"'" + notSingleQuote + "'";

            Repetition.Definition = (_)
                '{' + Expression + '}';

            Optional.Definition = (_)
                '[' + Expression + ']';

            Grouping.Definition = (_)
                '(' + Expression + ')';

            QualifiedIdentifier.Definition =
                identifier
                | (_)identifier + '.' + QualifiedIdentifier;

            Regex.Definition =
                RegexExpression
                | '^' + RegexExpression
                | RegexExpression + '$'
                | '^' + RegexExpression + '$';

            RegexExpression.Definition =
                RegexTerm
                | RegexTerm + '|' + RegexExpression
                | (_)null;

            RegexTerm.Definition =
                RegexFactor
                | RegexFactor + RegexTerm;

            RegexFactor.Definition =
                RegexAtom
                | RegexAtom + RegexIterator;

            RegexAtom.Definition =
                '.'
                | '(' + RegexExpression + ')'
                | RegexCharacter
                | RegexSet;

            RegexIterator.Definition = (_)
                '*'
                | '+'
                | '?';

            RegexSet.Definition =
                RegexPositiveSet
                | RegexNegativeSet;

            RegexPositiveSet.Definition =
                '[' + RegexCharacterClass + ']';

            RegexNegativeSet.Definition =
                "[^" + RegexCharacterClass + ']';

            RegexCharacterClass.Definition =
                RegexCharacterRange
                | RegexCharacterRange + RegexCharacterClass;

            RegexCharacterRange.Definition =
                RegexCharacterClassCharacter
                | RegexCharacterClassCharacter + '-' + RegexCharacterClassCharacter;

            RegexCharacter.Definition = (_)
                escapeCharacter
                | notMeta;

            RegexCharacterClassCharacter.Definition = (_)
                escapeCharacter
                | notCloseBracket;

            var grammarBuilder = new GrammarBuilder(
                Grammar,
                new[] { Grammar, Block, Rule, Setting, LexerRule, Expression, Term, Factor,
                    Grouping, Repetition, Optional, QualifiedIdentifier, Literal,
                    Regex, RegexExpression, RegexTerm, RegexFactor, RegexAtom,
                    RegexIterator, RegexSet, RegexPositiveSet, RegexNegativeSet,
                    RegexCharacterClass, RegexCharacterRange, RegexCharacter,
                    RegexCharacterClassCharacter},
                new[] { whitespace });

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
            return new DfaLexerRule(start, new TokenType(@"([^""]|(\\.))+"));
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

            return new DfaLexerRule(start, new TokenType(@"([^""]|(\\.))*"));
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