using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Grammars;
using Pliant.Lexemes;
using System.Collections.Generic;
using System.Linq;

namespace Pliant.Tests.Unit
{
    [TestClass]
    public class LexemeTests
    {
        [TestMethod]
        public void Test_Lexeme_That_Consumes_Whitespace()
        {
            var grammar = new GrammarBuilder("S", p => p
                    .Production("S", r=>r
                        .Rule("W")
                        .Rule("W", "S"))
                    .Production("W", r => r
                        .Rule(new WhitespaceTerminal())))
                .GetGrammar();

            var lexerRule = new LexerRule(
                new NonTerminal("whitespace"),
                grammar);

            var lexeme = new Lexeme(lexerRule);
            var input = "\t\r\n\v\f ";
            for (int i = 0; i < input.Length; i++)
                Assert.IsTrue(lexeme.Scan(input[i]));
            Assert.IsTrue(lexeme.IsAccepted());
        }

        [TestMethod]
        public void Test_Lexeme_That_Consumes_Character_Sequence()
        {
            var grammar = new GrammarBuilder("S", p => p
                    .Production("S", r => r
                        .Rule('a', 'b', 'c', '1', '2', '3')))
                .GetGrammar();

            var lexerRule = new LexerRule(
                new NonTerminal("sequence"),
                grammar);

            var lexeme = new Lexeme(lexerRule);
            var input = "abc123";
            for (int i = 0; i < input.Length; i++)
                Assert.IsTrue(lexeme.Scan(input[i]));
            Assert.IsTrue(lexeme.IsAccepted());
        }

        [TestMethod]
        public void Test_Lexeme_That_Matches_Longest_Acceptable_Token_When_Given_Ambiguity()
        {
            var lexemeList = new List<Lexeme>();

            var thereGrammar = new GrammarBuilder("S", p => p
                    .Production("S", r => r
                        .Rule('t', 'h', 'e', 'r', 'e')))
                .GetGrammar();
            var thereLexerRule = new LexerRule(
                new NonTerminal("there"), 
                thereGrammar);
            var thereLexeme = new Lexeme(thereLexerRule);
            lexemeList.Add(thereLexeme);

            var thereforeGrammar = new GrammarBuilder("S", p => p
                    .Production("S", r => r
                        .Rule('t', 'h', 'e', 'r', 'e', 'f', 'o', 'r', 'e')))
                .GetGrammar();
            var thereforeLexerRule = new LexerRule(
                new NonTerminal("therefore"), 
                thereforeGrammar);
            var thereforeLexeme = new Lexeme(thereforeLexerRule);
            lexemeList.Add(thereforeLexeme);
            
            var input = "therefore";
            var i = 0;
            for (; i < input.Length; i++)
            {
                var passedLexemes = lexemeList
                    .Where(l => l.Scan(input[i]))
                    .ToList();

                // all existing lexemes have failed
                // fall back onto the lexemes that existed before
                // we read this character
                if (passedLexemes.Count() == 0)
                    break;
                
                lexemeList = passedLexemes;
            }

            Assert.AreEqual(i, input.Length);
            Assert.AreEqual(1, lexemeList.Count);
            var remainingLexeme = lexemeList[0];
            Assert.IsNotNull(remainingLexeme);
            Assert.IsTrue(remainingLexeme.IsAccepted());
        }
    }
}
