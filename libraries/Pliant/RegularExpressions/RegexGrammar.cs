using Pliant.Automata;
using Pliant.Builders;
using Pliant.Grammars;
using Pliant.Tokens;
using System.Collections.Generic;

namespace Pliant.RegularExpressions
{
    public class RegexGrammar : IGrammar
    {
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
            var notMeta =  new NegationTerminal(
                       new SetTerminal('.', '^', '$', '(', ')', '[', ']', '+', '*', '?', '\\'));
            var notCloseBracket = new NegationTerminal(
                new CharacterTerminal(']'));
            var escape = CreateEscapeCharacterLexerRule();
 
            var regex = new ProductionBuilder("Regex");
            var expression = new ProductionBuilder("Expression");
            var term = new ProductionBuilder("Term");
            var factor = new ProductionBuilder("Factor");
            var atom = new ProductionBuilder("Atom");
            var iterator = new ProductionBuilder("Iterator");
            var set = new ProductionBuilder("Set");
            var positiveSet = new ProductionBuilder("PositiveSet");
            var negativeSet = new ProductionBuilder("NegativeSet");
            var characterClass = new ProductionBuilder("CharacterClass");
            var characterRange = new ProductionBuilder("CharacterRange");
            var character = new ProductionBuilder("Character");
            var characterClassCharacter = new ProductionBuilder("CharacterClassCharacter");

            var productions = new[] {
                regex, expression, term, factor, atom, iterator, set, positiveSet, negativeSet, characterClass,
                characterRange, character, characterClassCharacter };
            
            regex
                .Rule(expression)
                .Or('^', expression)
                .Or(expression, '$')
                .Or('^', expression, '$');
 
            expression
                .Rule(term)
                .Or(term, '|', expression)
                .Or();

            term
                .Rule(factor)
                .Or(factor, term);

            factor
                .Rule(atom)
                .Or(atom, iterator);

            atom
                .Rule('.')
                .Or(character)
                .Or('(', expression, ')')
                .Or(set);

            iterator
                .Rule('*')
                .Or('+')
                .Or('?');

            set
                .Rule(positiveSet)
                .Or(negativeSet);

            positiveSet
                .Rule('[', characterClass, ']');

            negativeSet
                .Rule("[^", characterClass, ']');

            characterClass
                .Rule(characterRange)
                .Or(characterRange, characterClass);

            characterRange
                .Rule(characterClassCharacter)
                .Or(characterClassCharacter, '-', characterClassCharacter);

            character
                .Rule(notMeta)
                .Or(escape);

            characterClassCharacter
                .Rule(notCloseBracket)
                .Or(escape);

            _regexGrammar = new GrammarBuilder(regex, productions)
                .ToGrammar();
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
