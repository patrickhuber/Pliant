using Pliant.Builders;
using Pliant.Grammars;
using Pliant.Tokens;
using System.Collections.Generic;

namespace Pliant.Regex
{
    public class RegexGrammar : IGrammar
    {
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

        private static IGrammar _regexGrammar;

        static RegexGrammar()
        {
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
             *  NegativeSet                ->   '[^' CharacterClass ']'
             *  
             *  CharacterClass             ->   CharacterRange |
             *                                  CharacterRange CharacterClass
             *  
             *  CharacterRange             ->   CharacterClassCharacter |
             *                                  CharacterClassCharacter '-' CharacterClassCharacter
             *  
             *  Character                  ->   NotMetaCharacter
             *                                  '\' AnyCharacter
             *                                  EscapeSequence
             *                                  
             *  CharacterClassCharacter    ->   NotCloseBracketCharacter | 
             *                                  '\' AnyCharacter
             */
            const string Regex = "Regex";
            const string Expression = "Expression";
            const string Term = "Term";
            const string Factor = "Factor";
            const string Atom = "Atom";
            const string Iterator = "Iterator";
            const string Set = "Set";
            const string PositiveSet = "PositiveSet";
            const string NegativeSet = "NegativeSet";
            const string CharacterClass = "CharacterClass";
            const string Character = "Character";
            const string CharacterRange = "CharacterRange";
            const string CharacterClassCharacter = "CharacterClassCharacter";
            const string NotCloseBracket = "NotCloseBracket";
            const string NotMetaCharacter = "NotMetaCharacter";
            
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
            var notCloseBracket = new TerminalLexerRule(new NegationTerminal(new Terminal(']')), new TokenType("!]"));
            var dash = new TerminalLexerRule('-');
            var backslash = new TerminalLexerRule('\\');
            var notMeta = new TerminalLexerRule(
                new NegationTerminal(
                    new SetTerminal('.', '^', '$', '(', ')', '[', ']', '+', '*', '?', '\\')), 
                new TokenType("not-meta"));
            var any = new TerminalLexerRule(new AnyTerminal(), new TokenType("any"));

            var grammarBuilder = new GrammarBuilder(Regex)
                .Production(Regex, r => r
                    .Rule(Expression)
                    .Rule(caret, Expression)
                    .Rule(Expression, dollar)
                    .Rule(caret, Expression, dollar))
                .Production(Expression, r => r
                    .Rule(Term)
                    .Rule(Term, pipe, Expression)
                    .Lambda())
                .Production(Term, r => r
                    .Rule(Factor)
                    .Rule(Factor, Term))
                .Production(Factor, r => r
                    .Rule(Atom)
                    .Rule(Atom, Iterator))
                .Production(Atom, r => r
                    .Rule(dot)
                    .Rule(Character)
                    .Rule(openParen, Expression, closeParen)
                    .Rule(Set))
                .Production(Iterator, r => r
                    .Rule(star)
                    .Rule(plus)
                    .Rule(question))
                .Production(Set, r => r
                    .Rule(PositiveSet)
                    .Rule(NegativeSet))
                .Production(PositiveSet, r => r
                    .Rule(openBracket, CharacterClass, closeBracket))
                .Production(NegativeSet, r => r
                    .Rule(openBracket, caret, CharacterClass, closeBracket))
                .Production(CharacterClass, r => r
                    .Rule(CharacterRange)
                    .Rule(CharacterRange, CharacterClass))
                .Production(CharacterRange, r => r
                    .Rule(CharacterClassCharacter)
                    .Rule(CharacterClassCharacter, dash, CharacterClassCharacter))
                .Production(Character, r => r
                    .Rule(NotMetaCharacter)
                    .Rule(backslash, any))
                .Production(CharacterClassCharacter, r => r
                    .Rule(NotCloseBracket)
                    .Rule(backslash, any))
                .Production(NotMetaCharacter, r => r
                    .Rule(notMeta))
                .Production(NotCloseBracket, r => r
                    .Rule(notCloseBracket));
            _regexGrammar = grammarBuilder.ToGrammar();
        }
    }
}
