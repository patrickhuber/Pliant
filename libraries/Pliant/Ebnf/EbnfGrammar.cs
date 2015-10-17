using Pliant.Automata;
using Pliant.Grammars;
using Pliant.Tokens;
using System.Collections.Generic;
using System;
using Pliant.Builders;

namespace Pliant.Ebnf
{
    public class EbnfGrammar : IGrammar
    {
        private static readonly IGrammar _ebnfGrammar;

        static void EbnfGrammar2()
        {
            ProductionBuilder 
                Grammar = "Grammar", 
                RuleOrSetting = "RuleOrSetting", 
                Rule = "Rule",
                Setting = "Setting";

            Grammar.Definition = 
                RuleOrSetting 
                | RuleOrSetting + Grammar;

            RuleOrSetting.Definition = 
                Rule 
                | Setting;

            var grammarBuilder = new GrammarBuilder(
                Grammar,
                new[] { Grammar, RuleOrSetting, Rule, Setting });
            IGrammar g = grammarBuilder.ToGrammar();
        }

        static EbnfGrammar()
        {
            var whitespace = CreateWhitespaceLexerRule();
            var notDoubleQuote = CreateNotDoubleQuoteLexerRule();
            var notSingleQuuote = CreateNotSingleQuoteLexerRule();
            var identifier = CreateIdentifierLexerRule();
            var settingIdentifier = CreateSettingIdentifierLexerRule();
            var escapeCharacter = CreateEscapeCharacterLexerRule();

            var grammar = new NonTerminal("Grammar");
            var rule = new NonTerminal("Rule");
            var setting = new NonTerminal("Setting");
            var ruleOrSetting = new NonTerminal("RuleOrSetting");
            var expression = new NonTerminal("Expression");
            var term = new NonTerminal("Term");
            var factor = new NonTerminal("Factor");
            var atom = new NonTerminal("Atom");
            var literal = new NonTerminal("Literal");
            var doubleQuoteText = new NonTerminal("DoubleQuoteText");
            var singleQuoteText = new NonTerminal("SingleQuoteText");
            var qualifiedIdentifier = new NonTerminal("QualifiedIdentifier");
            var grouping = new NonTerminal("Grouping");
            var optional = new NonTerminal("Optional");
            var repetition = new NonTerminal("Repetition");
            var regex = new NonTerminal("Regex");
            var regexExpression = new NonTerminal("Regex.Expression");
            var regexTerm = new NonTerminal("Regex.Term");
            var regexFactor = new NonTerminal("Regex.Factor");
            var regexAtom = new NonTerminal("Regex.Atom");
            var regexIterator = new NonTerminal("Regex.Iterator");
            var regexCharacter = new NonTerminal("Regex.Character");
            var regexSet = new NonTerminal("Regex.Set");
            var regexPositiveSet = new NonTerminal("Regex.PositiveSet");
            var regexNegativeSet = new NonTerminal("Regex.NegativeSet");
            var regexCharacterClass = new NonTerminal("Regex.CharacterClass");
            var regexCharacterRange = new NonTerminal("Regex.CharacterRange");
            var regexCharacterClassCharacter = new NonTerminal("Regex.CharacterClassCharacter");
            
            var productions = new[]
            {
                /*  Grammar         
                        = RuleOrSetting { RuleOrSetting } ;
                 */
                new Production(grammar, ruleOrSetting),
                new Production(grammar, ruleOrSetting, grammar),
                
                /*  RuleOrSetting
                        = Rule | Setting
                 */
                new Production(ruleOrSetting, rule),
                new Production(ruleOrSetting, setting),

                /*  Rule
                        = QualifiedIdentifier '=' Expression ';' ;
                 */
                new Production(rule, qualifiedIdentifier, new TerminalLexerRule('='), expression, new TerminalLexerRule(';')),
                
                /*  Setting
                        = r":[a-zA-Z][a-zA-Z0-9]*" '=' QualifiedIdentifier ';' ;
                 */
                new Production(setting, settingIdentifier, new TerminalLexerRule('='), qualifiedIdentifier, new TerminalLexerRule(';')),

                /*  Expression 
                        = Term
                        | Term '|' Expression ;
                 */
                new Production(expression, term),
                new Production(expression, term, new TerminalLexerRule('|'), expression),

                /*  Term
                        = Factor
                        | Factor Term ;
                 */
                new Production(term, factor),
                new Production(term, factor, term),

                /*  Grouping
                       = '(' Expression ')' ;
                 */
                new Production(grouping, new TerminalLexerRule('('), expression, new TerminalLexerRule(')')),

                /*  Repetition
                        = '{' Expression '}' ;
                 */
                new Production(repetition, new TerminalLexerRule('{'), expression, new TerminalLexerRule('}')),

                /*  Optional 
                        = '[' Expression ']' ;
                 */
                new Production(optional, new TerminalLexerRule('['), expression, new TerminalLexerRule(']')),

                /*  Factor
                        = QualifiedIdentifier
                        | Literal
                        | '/' Regex '/'
                        | Repetition
                        | Optional
                        | Grouping ;
                 */
                new Production(factor, qualifiedIdentifier),
                new Production(factor, literal),
                new Production(factor, new TerminalLexerRule('r'), new TerminalLexerRule('"'), regex, new TerminalLexerRule('"')),
                new Production(factor, repetition),
                new Production(factor, optional),
                new Production(factor, grouping),

                //  QualifiedIdentifier
                //        = /[a-zA-Z0-9][a-zA-Z0-9_-]*/ ;
                //
                new Production(qualifiedIdentifier, identifier),
                new Production(qualifiedIdentifier, identifier, new TerminalLexerRule('.'), qualifiedIdentifier),

                /*  Literal 
                        = '"' /[^""]/ '"'
                        | ""'"" /[^']/ ""'"" ;
                */
                new Production(literal, new TerminalLexerRule('"'), notDoubleQuote, new TerminalLexerRule('"')),
                new Production(literal, new TerminalLexerRule('\''), notSingleQuuote, new TerminalLexerRule('\'')),

                /*  Regex 
                        = Regex.Expression
                        | '^' Regex.Expression
                        | Regex.Expression '$'
                        | '^' Regex.Expression '$' ;
                 */
                new Production(regex, regexExpression),
                new Production(regex, new TerminalLexerRule('^'), regexExpression),
                new Production(regex, new TerminalLexerRule('^'), regexExpression, new TerminalLexerRule('$')),
                new Production(regex, regexExpression, new TerminalLexerRule('$')),

                /*  Regex.Expression 
                        = [ Regex.Term 
                        | Regex.Term '|' Regex.Expression ] ;
                */
                new Production(regexExpression, regexTerm),
                new Production(regexExpression, regexTerm, new TerminalLexerRule('|'), regexExpression),
                new Production(regexExpression),

                /*  Regex.Term
                        = Regex.Factor
                        | Regex.Factor Regex.Term ;
                 */
                new Production(regexTerm, regexFactor),
                new Production(regexTerm, regexFactor, regexTerm),

                /*  Regex.Factor
                        = Regex.Atom
                        | Regex.Atom Regex.Iterator ;
                 */
                new Production(regexFactor, regexAtom),
                new Production(regexFactor, regexAtom, regexIterator),

                /*  Regex.Atom
                        = '.'
                        | '(' Rege.Expression ')'
                        | Regex.Character
                        | Regex.Set ;
                 */
                new Production(regexAtom, new TerminalLexerRule('.')),
                new Production(regexAtom, new TerminalLexerRule('('), regexExpression, new TerminalLexerRule(')')),
                new Production(regexAtom, regexCharacter),
                new Production(regexAtom, regexSet),

                /*  Regex.Iterator 
                        = '*' | '+' | '?' ;
                 */
                new Production(regexIterator, new TerminalLexerRule('*')),
                new Production(regexIterator, new TerminalLexerRule('+')),
                new Production(regexIterator, new TerminalLexerRule('?')),

                /*  Regex.Set 
                        = Regex.PositiveSet
                        | Regex.NegativeSet ;
                 */
                new Production(regexSet, regexNegativeSet),
                new Production(regexSet, regexPositiveSet),

                /*  Regex.PositiveSet 
                        = '[' Regex.CharacterClass ']' ;
                */
                new Production(regexPositiveSet, new TerminalLexerRule('['), regexCharacterClass, new TerminalLexerRule(']')),
                
                /*  Regex.NegativeSet 
                        = "[^" Regex.CharacterClass ']' ;
                */
                new Production(regexNegativeSet, new StringLiteralLexerRule("[^"), regexCharacterClass, new TerminalLexerRule(']')),

                /*  Regex.CharacterClass
                        = Regex.CharacterRange
                        | Regex.CharacterRange Regex.CharacterClass;
                 */
                new Production(regexCharacterClass, regexCharacterRange),
                new Production(regexCharacterClass, regexCharacterRange, regexCharacterClass),

                /*  Regex.CharacterRange
                        = Regex.CharacterClassCharacter
                        | Regex.CharacterClassCharacter '-' Regex.CharacterClassCharacter;
                */
                new Production(regexCharacterRange, regexCharacterClassCharacter),
                new Production(regexCharacterRange, regexCharacterClassCharacter, new TerminalLexerRule('-'), regexCharacterClassCharacter),

                /*  Regex.Character
                        = /[^.^$()[\]+*?\\]/
                        | '\\' /./ ;
                 */
                new Production(
                    regexCharacter,
                    new TerminalLexerRule(
                        new NegationTerminal(
                            new SetTerminal('.', '^', '$', '(', ')', '[', ']', '+', '*', '?', '\\')),
                        "notMeta")),
                new Production(
                    regexCharacter, new TerminalLexerRule('\\'), new TerminalLexerRule(new AnyTerminal(), "any")),

                /*  Regex.CharacterClassCharacter
                        = /[^\]]/
                        | '\\' /./ ;
                 */
                new Production(regexCharacterClassCharacter,
                    new TerminalLexerRule(
                        new NegationTerminal(new CharacterTerminal(']')), "[^\\]]")),
                new Production(regexCharacterClassCharacter,
                    escapeCharacter)                
            };

            var ignore = new[]
            {
                whitespace
            };

            _ebnfGrammar = new Grammar(grammar, productions, ignore);
        }

        private static ILexerRule CreateEscapeCharacterLexerRule()
        {
            var start = new DfaState();
            var escape = new DfaState();
            var final = new DfaState(true);
            start.AddTransition(new DfaTransition(new CharacterTerminal('\\'), escape));
            escape.AddTransition(new DfaTransition(new AnyTerminal(), final));
            return new DfaLexerRule(start, "escape");
        }

        private static ILexerRule CreateNotSingleQuoteLexerRule()
        {
            var start = new DfaState();
            var final = new DfaState(true);
            var terminal = new NegationTerminal(new CharacterTerminal('\''));
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
            var escapeTerminal = new CharacterTerminal('\\');
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

        private static ILexerRule CreateSettingIdentifierLexerRule()
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

        private static ILexerRule CreateIdentifierLexerRule()
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

        private static ILexerRule CreateWhitespaceLexerRule()
        {
            var whitespaceTerminal = new WhitespaceTerminal();
            var startWhitespace = new DfaState();
            var finalWhitespace = new DfaState(true);
            startWhitespace.AddTransition(new DfaTransition(whitespaceTerminal, finalWhitespace));
            finalWhitespace.AddTransition(new DfaTransition(whitespaceTerminal, finalWhitespace));
            var whitespace = new DfaLexerRule(startWhitespace, "whitespace");
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
