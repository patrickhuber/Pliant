using Pliant.Automata;
using Pliant.Builders;
using Pliant.Builders.Expressions;
using Pliant.Grammars;
using System.Collections.Generic;

namespace Pliant.RegularExpressions
{    
    public class RegexGrammar : IGrammar
    {
        private readonly static IGrammar _regexGrammar;

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

        public static readonly string Namespace = nameof(RegularExpressions);
        public static readonly FullyQualifiedName Regex = new FullyQualifiedName(Namespace, nameof(Regex));
        public static readonly FullyQualifiedName Expression = new FullyQualifiedName(Namespace, nameof(Expression));
        public static readonly FullyQualifiedName Term = new FullyQualifiedName(Namespace, nameof(Term));
        public static readonly FullyQualifiedName Factor = new FullyQualifiedName(Namespace, nameof(Factor));
        public static readonly FullyQualifiedName Atom = new FullyQualifiedName(Namespace, nameof(Atom));
        public static readonly FullyQualifiedName Iterator = new FullyQualifiedName(Namespace, nameof(Iterator));
        public static readonly FullyQualifiedName Set = new FullyQualifiedName(Namespace, nameof(Set));
        public static readonly FullyQualifiedName PositiveSet = new FullyQualifiedName(Namespace, nameof(PositiveSet));
        public static readonly FullyQualifiedName NegativeSet = new FullyQualifiedName(Namespace, nameof(NegativeSet));
        public static readonly FullyQualifiedName CharacterClass = new FullyQualifiedName(Namespace, nameof(CharacterClass));
        public static readonly FullyQualifiedName CharacterRange = new FullyQualifiedName(Namespace, nameof(CharacterRange));
        public static readonly FullyQualifiedName Character = new FullyQualifiedName(Namespace, nameof(Character));
        public static readonly FullyQualifiedName CharacterClassCharacter = new FullyQualifiedName(Namespace, nameof(CharacterClassCharacter));
        
        static RegexGrammar()
        {
            var notMeta = CreateNotMetaLexerRule();
            var notCloseBracket = CreateNotCloseBracketLexerRule();
            var escape = CreateEscapeCharacterLexerRule();

            ProductionExpression
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
            
            regex.Rule
                = expression
                | '^' + expression
                | expression + '$'
                | '^' + expression + '$';

            expression.Rule
                = term
                | term + '|' + expression;

            term.Rule
                = factor
                | factor + term;

            factor.Rule
                = atom
                | atom + iterator;

            atom.Rule
                = '.'
                | character
                | '(' + expression + ')'
                | set;

            iterator.Rule = (Expr)
                '*'
                | '+'
                | '?';

            set.Rule
                = positiveSet
                | negativeSet;

            positiveSet.Rule
                = '[' + characterClass + ']';

            negativeSet.Rule
                = "[^" + characterClass + ']';

            characterClass.Rule
                = characterRange
                | characterRange + characterClass;

            characterRange.Rule
                = characterClassCharacter
                | characterClassCharacter + '-' + characterClassCharacter;

            character.Rule = (Expr)
                notMeta
                | escape;

            characterClassCharacter.Rule = (Expr)
                notCloseBracket
                | escape;

            _regexGrammar = new GrammarExpression(
                regex, 
                new[] 
                {
                    regex,
                    expression,
                    term,
                    factor,
                    atom,
                    iterator,
                    set,
                    positiveSet,
                    negativeSet,
                    characterClass,
                    characterRange,
                    character,
                    characterClassCharacter
                })
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
        
        public bool IsNullable(INonTerminal nonTerminal)
        {
            return _regexGrammar.IsNullable(nonTerminal);
        }
    }
}