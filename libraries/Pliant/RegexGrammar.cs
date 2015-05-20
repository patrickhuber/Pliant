using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant
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

        public IEnumerable<ILexerRule> LexerRulesFor(INonTerminal nonTerminal)
        {
            return _regexGrammar.LexerRulesFor(nonTerminal);
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
             *              
             *  Term                       ->   Factor |   
             *                                  Factor Term
             *             
             *  Factor                     ->   Atom |   
             *                                  Atom Iterator
             *  
             *  Atom                       ->   . |
             *                                  Char | 
             *                                  '(' Expression ')' | 
             *                                  Set
             *  
             *  Set                        ->   PositiveSet |
             *                                  NegativeSet
             *  
             *  PositiveSet                ->   '[' CharacterSet ']'
             *  
             *  NegativeSet                ->   '[^' CharacterSet ']'
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

            var grammarBuilder = new GrammarBuilder(Regex, p => p
                .Production(Regex, r => r
                    .Rule(Expression)
                    .Rule('^', Expression)
                    .Rule(Expression, '$')
                    .Rule('^', Expression, '$'))
                .Production(Expression, r => r
                    .Rule(Term)
                    .Rule(Term, '|', Expression))
                .Production(Term, r => r
                    .Rule(Factor)
                    .Rule(Factor, Term))
                .Production(Factor, r => r
                    .Rule(Atom)
                    .Rule(Atom, Iterator))
                .Production(Atom, r => r
                    .Rule('.')
                    .Rule(Character)
                    .Rule('(', Expression, ')')
                    .Rule(Set)
                    .Lambda())
                .Production(Iterator, r => r
                    .Rule(new SetTerminal('*', '+', '?')))
                .Production(Set, r => r
                    .Rule(PositiveSet)
                    .Rule(NegativeSet))
                .Production(PositiveSet, r => r
                    .Rule('[', CharacterClass, ']'))
                .Production(NegativeSet, r => r
                    .Rule('[', '^', CharacterClass, ']'))
                .Production(CharacterClass, r => r
                    .Rule(CharacterRange)
                    .Rule(CharacterRange, CharacterClass))
                .Production(CharacterRange, r => r
                    .Rule(CharacterClassCharacter)
                    .Rule(CharacterClassCharacter, '-', CharacterClassCharacter))
                .Production(Character, r => r
                    .Rule(NotMetaCharacter)
                    .Rule('\\', new AnyTerminal()))
                .Production(CharacterClassCharacter, r => r
                    .Rule(NotCloseBracket)
                    .Rule('\\', new AnyTerminal()))
                .Production(NotMetaCharacter, r => r
                    .Rule(new NegationTerminal(new SetTerminal('.', '^', '$', '(', ')', '[', ']', '+', '*', '?'))))
                .Production(NotCloseBracket, r => r
                    .Rule(new NegationTerminal(new Terminal(']')))));
            _regexGrammar = grammarBuilder.GetGrammar();
        }
    }
}
