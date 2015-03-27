using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Pliant.Tests.Unit
{
    /// <summary>
    /// Summary description for RegexTests
    /// </summary>
    [TestClass]
    public class RegexTests
    {
        PulseRecognizer _pulseRecognizer;
        IGrammar _regexGrammar;

        [TestInitialize]
        public void Initialize_Regex_Tests()
        {
            _pulseRecognizer = new PulseRecognizer(_regexGrammar);
        }

        public RegexTests()
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
             *  Atom                       ->   Char | 
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
                    .Rule(Character)
                    .Rule('(', Expression, ')')
                    .Rule(Set))
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
                    .Rule(new NegationTerminal(new SetTerminal('.', '^', '$'))))
                .Production(NotCloseBracket, r => r
                    .Rule(new NegationTerminal(new Terminal(']')))));
            _regexGrammar = grammarBuilder.GetGrammar();
        }

        public TestContext TestContext { get; set; }
        
        [TestMethod]
        public void Test_Regex_That_Parses_Single_Character()
        {
            var input = "a";
            Recognize(input);
        }

        [TestMethod]
        public void Test_Regex_That_Parses_String_Literal()
        {
            var input = "abc";
            Recognize(input);
        }

        [TestMethod]
        public void Test_Regex_That_Parses_Whitespace_Character_Class()
        {
            var input = @"[\s]+";
            Recognize(input);
        }

        [TestMethod]
        public void Test_Regex_That_Parses_Range_Character_Class()
        {
            var input = @"[a-z]";
            Recognize(input);
        }

        private void Recognize(string input)
        {
            foreach (var c in input)
                Assert.IsTrue(_pulseRecognizer.Pulse(c),
                    string.Format("Line 0, Column {1} : Invalid Character '{0}'",
                        c,
                        _pulseRecognizer.Location));
            Assert.IsTrue(_pulseRecognizer.IsAccepted());
        }
    }
}
