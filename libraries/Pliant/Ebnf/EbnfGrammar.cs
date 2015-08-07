using Pliant.Automata;
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
            /*  Grammar             = { Rule } ;
                Rule                = RuleName '=' Expression ';' ;
                RuleName            = Identifier ;
                Expression          = Identifier
                                    | Terminal
                                    | '[' Expression ']'
                                    | '{' Expression '}'
                                    | '(' Expression ')'
                                    | Expression '|' Expression
                                    | Expression Expression ;
                Identifier          = Letter { Letter | Digit | '_' };
                Terminal            = '"' String '"'
                                    | "'" Character "'"
                                    | 'r' '"' Regex '"' ; 
                String              = { StringCharacter } ;
                StringCharacter     = r"[^\"]"
                                    | '\\' AnyCharacter ;
                Character           = SingleCharacter
                                    | '\\' SimpleEscape ;
                SingleCharacter     = r"[^']";
                SimpleEscape        = "'" | '"' | '\\' | '0' | 'a' | 'b' 
                                    | 'f' | 'n'  | 'r' | 't' | 'v' ;
                Digit               = r"[0-9]" ;
                Letter              = r"[a-zA-Z]";
                Whitespace          = r"\w+";

                Regex               = ['^'] RegexExpression ['$']
                RegexExpression     = [ RegexTerm ]
                                    | RegexTerm '|' RegexExpression
                RegexTerm           = RegexFactor [ RegexTerm ]
                RegexFactor         = RegexAtom [ RegexIterator ]
                RegexAtom           = '.'
                                    | RegexCharacter
                                    | '(' RegexExpression ')'
                                    | RegexSet
                RegexSet            = PositiveSet
                                    | NegativeSet
                PositiveSet         = '[' CharacterClass ']'
                                    | "[^" CharacterClass "]"
                CharacterClass      = CharacterRange { CharacterRange }
                :ignore             = Whitespace;
            */
            var whitespace = CreateWhitespaceLexerRule();
            var grammar = new NonTerminal("grammar");
            var rule = new NonTerminal("rule");
            var grammarRule = new NonTerminal("grammarRule");

            var productions = new[] 
            {
                new Production(grammar, grammarRule),
                new Production(grammarRule, rule),
                new Production(grammarRule),
            };

            var ignore = new[]
            {
                whitespace
            };  
                 
            _ebnfGrammar = new Grammar(grammar, productions, ignore);
        }

        private static ILexerRule CreateWhitespaceLexerRule()
        {
            var whitespaceTerminal = new WhitespaceTerminal();
            var startWhitespace = new DfaState();
            var finalWhitespace = new DfaState(true);
            startWhitespace.AddEdge(new DfaEdge(whitespaceTerminal, finalWhitespace));
            finalWhitespace.AddEdge(new DfaEdge(whitespaceTerminal, finalWhitespace));
            var whitespace = new DfaLexerRule(startWhitespace, new TokenType("whitespace"));
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
