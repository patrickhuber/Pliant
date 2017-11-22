using Pliant.Automata;
using Pliant.Grammars;

namespace Pliant.RegularExpressions
{
    public class RegexGrammar : GrammarWrapper
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

            var regex = new NonTerminal(Regex);
            var expression = new NonTerminal(Expression);
            var term = new NonTerminal(Term);
            var factor = new NonTerminal(Factor);
            var atom = new NonTerminal(Atom);
            var iterator = new NonTerminal(Iterator);
            var set = new NonTerminal(Set);
            var positiveSet = new NonTerminal(PositiveSet);
            var negativeSet = new NonTerminal(NegativeSet);
            var characterClass = new NonTerminal(CharacterClass);
            var characterRange = new NonTerminal(CharacterRange);
            var character = new NonTerminal(Character);
            var characterClassCharacter = new NonTerminal(CharacterClassCharacter);

            var caret = new TerminalLexerRule('^');
            var dollar = new TerminalLexerRule('$');
            var pipe = new TerminalLexerRule('|');
            var dot = new TerminalLexerRule('.');
            var openParen = new TerminalLexerRule('(');
            var closeParen = new TerminalLexerRule(')');
            var star = new TerminalLexerRule('*');
            var plus = new TerminalLexerRule('+');
            var question = new TerminalLexerRule('?');
            var openBracket = new TerminalLexerRule('[');
            var closeBracket = new TerminalLexerRule(']');
            var minus = new TerminalLexerRule('-');

            var productions = new[]
            {
                new Production(regex, expression),
                new Production(regex, caret, expression),
                new Production(regex, expression, dollar),
                new Production(regex, caret, expression, dollar),
                new Production(expression, term),
                new Production(expression, term, pipe, expression),
                new Production(term, factor),
                new Production(term, factor, term),
                new Production(factor, atom),
                new Production(factor, atom, iterator),
                new Production(atom, dot),
                new Production(atom, character),
                new Production(atom, openParen, expression, closeParen),
                new Production(atom, set),
                new Production(iterator, star),
                new Production(iterator, plus),
                new Production(iterator, question),
                new Production(set, positiveSet),
                new Production(set, negativeSet),
                new Production(negativeSet, openBracket, caret, characterClass, closeBracket),
                new Production(positiveSet, openBracket, characterClass, closeBracket),                
                new Production(characterClass, characterRange),
                new Production(characterClass, characterRange, characterClass),
                new Production(characterRange, characterClassCharacter),
                new Production(characterRange, characterClassCharacter, minus, characterClassCharacter),
                new Production(character, notMeta),
                new Production(character, escape),
                new Production(characterClassCharacter, notCloseBracket),
                new Production(characterClassCharacter, escape)
            };

            _regexGrammar = new Grammar(regex, productions, null, null);
        }
        
        private static BaseLexerRule CreateNotMetaLexerRule()
        {
            return new TerminalLexerRule(
                new NegationTerminal(
                       new SetTerminal('.', '^', '$', '(', ')', '[', ']', '+', '*', '?', '\\', '/')),
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

        public RegexGrammar()
            : base(_regexGrammar)
        {
        }
    }
}