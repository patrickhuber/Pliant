using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Builders;
using Pliant.Grammars;
using Pliant.Lexemes;
using Pliant.Tokens;
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
                .ToGrammar();

            var lexerRule = new GrammarLexerRule(
                "whitespace",
                grammar);

            var parseEngine = new ParseEngine(lexerRule.Grammar);
            var lexeme = new ParseEngineLexeme(parseEngine, new TokenType("whitespace"));
            var input = "\t\r\n\v\f ";
            for (int i = 0; i < input.Length; i++)
                Assert.IsTrue(lexeme.Scan(input[i]));
            Assert.IsTrue(lexeme.IsAccepted());
        }

        [TestMethod]
        public void Test_Lexeme_That_Consumes_Character_Sequence()
        {
            var grammar = new GrammarBuilder("sequence", p => p
                    .Production("sequence", r => r
                        .Rule('a', 'b', 'c', '1', '2', '3')))
                .ToGrammar();

            
            var parseEngine = new ParseEngine(grammar);
            var lexeme = new ParseEngineLexeme(parseEngine, new TokenType("sequence"));
            var input = "abc123";
            for (int i = 0; i < input.Length; i++)
                Assert.IsTrue(lexeme.Scan(input[i]));
            Assert.IsTrue(lexeme.IsAccepted());
        }

        [TestMethod]
        public void Test_Lexeme_That_Matches_Longest_Acceptable_Token_When_Given_Ambiguity()
        {
            var lexemeList = new List<ParseEngineLexeme>();

            const string There = "there";
            var thereGrammar = new GrammarBuilder(There, p => p
                    .Production(There, r => r
                        .Rule('t', 'h', 'e', 'r', 'e')))
                .ToGrammar();
            var thereParseEngine = new ParseEngine(thereGrammar);
            var thereLexeme = new ParseEngineLexeme(thereParseEngine, new TokenType(There));
            lexemeList.Add(thereLexeme);

            const string Therefore = "therefore";
            var thereforeGrammar = new GrammarBuilder(Therefore, p => p
                    .Production(Therefore, r => r
                        .Rule('t', 'h', 'e', 'r', 'e', 'f', 'o', 'r', 'e')))
                .ToGrammar();
            var parseEngine = new ParseEngine(thereforeGrammar);
            var thereforeLexeme = new ParseEngineLexeme(parseEngine, new TokenType(Therefore));
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
