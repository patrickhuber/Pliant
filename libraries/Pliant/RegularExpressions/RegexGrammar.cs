using Pliant.Automata;
using Pliant.Builders;
using Pliant.Grammars;
using System.Collections.Generic;

namespace Pliant.RegularExpressions
{    
    public class RegexGrammar : IGrammar
    {
        private static IGrammar _regexGrammar;

        /*  Regex                      ->   Expression |
         *                                  '^' Expression |
         *                                  Expression '$' |
         *                                  '^' Expression '$'
         *
         *  Expresion                  ->   Term |
         *                                  Term '|' Expression
         *                                  λ
         *
         *  Term                       ->   Factor |
         *                                  Factor Term
         *
         *  Factor                     ->   Atom |
         *                                  Atom Iterator
         *
         *  Atom                       ->   . |
         *                                  Character |
         *                                  '(' Expression ')' |
         *                                  Set
         *
         *  Set                        ->   PositiveSet |
         *                                  NegativeSet
         *
         *  PositiveSet                ->   '[' CharacterClass ']'
         *
         *  NegativeSet                ->   "[^" CharacterClass ']'
         *
         *  CharacterClass             ->   CharacterRange |
         *                                  CharacterRange CharacterClass
         *
         *  CharacterRange             ->   CharacterClassCharacter |
         *                                  CharacterClassCharacter '-' CharacterClassCharacter
         *
         *  Character                  ->   NotMetaCharacter |
         *                                  EscapeSequence
         *
         *  CharacterClassCharacter    ->   NotCloseBracketCharacter |
         *                                  EscapeSequence
         */

        public static readonly string Namespace = "RegularExpressions";
        public static readonly FullyQualifiedName Regex = new FullyQualifiedName(Namespace, "Regex");
        public static readonly FullyQualifiedName Expression = new FullyQualifiedName(Namespace, "Expression");
        public static readonly FullyQualifiedName Term = new FullyQualifiedName(Namespace, "Term");
        public static readonly FullyQualifiedName Factor = new FullyQualifiedName(Namespace, "Factor");
        public static readonly FullyQualifiedName Atom = new FullyQualifiedName(Namespace, "Atom");
        public static readonly FullyQualifiedName Iterator = new FullyQualifiedName(Namespace, "Iterator");
        public static readonly FullyQualifiedName Set = new FullyQualifiedName(Namespace, "Set");
        public static readonly FullyQualifiedName PositiveSet = new FullyQualifiedName(Namespace, "PositiveSet");
        public static readonly FullyQualifiedName NegativeSet = new FullyQualifiedName(Namespace, "NegativeSet");
        public static readonly FullyQualifiedName CharacterClass = new FullyQualifiedName(Namespace, "CharacterClass");
        public static readonly FullyQualifiedName CharacterRange = new FullyQualifiedName(Namespace, "CharacterRange");
        public static readonly FullyQualifiedName Character = new FullyQualifiedName(Namespace, "Character");
        public static readonly FullyQualifiedName CharacterClassCharacter = new FullyQualifiedName(Namespace, "CharacterClassCharacter");
        
        static RegexGrammar()
        {
            var notMeta = CreateNotMetaLexerRule();
            var notCloseBracket = CreateNotCloseBracketLexerRule();
            var escape = CreateEscapeCharacterLexerRule();

            ProductionBuilder
                regex = Regex,
                expression = Expression,
                term = Term,
                factor = Factor,
                atom = Atom,
                iterator = Iterator,
                set = Set,
                positiveSet = PositiveSet,
                negativeSet = NegativeSet,
                characterClass = CharacterClass,
                characterRange = CharacterRange,
                character = Character,
                characterClassCharacter = CharacterClassCharacter;
            
            regex.Definition
                = expression
                | '^' + expression
                | expression + '$'
                | '^' + expression + '$';

            expression.Definition
                = term
                | term + '|' + expression;

            term.Definition
                = factor
                | factor + term;

            factor.Definition
                = atom
                | atom + iterator;

            atom.Definition
                = '.'
                | character
                | '(' + expression + ')'
                | set;

            iterator.Definition = (_)
                '*'
                | '+'
                | '?';

            set.Definition
                = positiveSet
                | negativeSet;

            positiveSet.Definition
                = '[' + characterClass + ']';

            negativeSet.Definition
                = "[^" + characterClass + ']';

            characterClass.Definition
                = characterRange
                | characterRange + characterClass;

            characterRange.Definition
                = characterClassCharacter
                | characterClassCharacter + '-' + characterClassCharacter;

            character.Definition = (_)
                notMeta
                | escape;

            characterClassCharacter.Definition = (_)
                notCloseBracket
                | escape;

            _regexGrammar = new GrammarBuilder(regex)
                .ToGrammar();
        }

        private static BaseLexerRule CreateNotMetaLexerRule()
        {
            return new TerminalLexerRule(
                new NegationTerminal(
                       new SetTerminal('.', '^', '$', '(', ')', '[', ']', '+', '*', '?', '\\')),
                "NotMeta");
        }

        private static BaseLexerRule CreateNotCloseBracketLexerRule()
        {
            return new TerminalLexerRule(
                new NegationTerminal(
                    new CharacterTerminal(']')),
                "NotCloseBracket");
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

        public IReadOnlyList<IProduction> Productions
        {
            get { return _regexGrammar.Productions; }
        }

        public INonTerminal Start
        {
            get { return _regexGrammar.Start; }
        }

        public IReadOnlyList<ILexerRule> Ignores
        {
            get { return _regexGrammar.Ignores; }
        }

        public IEnumerable<IProduction> RulesFor(INonTerminal nonTerminal)
        {
            return _regexGrammar.RulesFor(nonTerminal);
        }

        public IEnumerable<IProduction> StartProductions()
        {
            return _regexGrammar.StartProductions();
        }
    }
}