using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Pliant.Tests.Unit
{
    [TestClass]
    public class ParseRunnerTests
    {
        private IGrammar _grammar;

        public ParseRunnerTests()
        {
            var whitespaceRule = CreateWhitespaceRule();
            var wordRule = CreateWordRule();
            CreateGrammar(whitespaceRule, wordRule);
        }

        private void CreateGrammar(ILexerRule whitespaceRule, ILexerRule wordRule)
        {
            _grammar = new GrammarBuilder("S", p => p
                    .Production("S", r => r
                        .Rule(whitespaceRule)
                        .Rule(whitespaceRule, "S")
                        .Rule(wordRule)
                        .Rule(wordRule, "S")))
                .GetGrammar();
        }

        private ILexerRule CreateWhitespaceRule()
        {
            var whitespaceGrammar = new GrammarBuilder("S", p => p
                    .Production("S", r => r
                        .Rule("whitespace")
                        .Rule("whitespace", "S"))
                    .Production("whitespace", r=>r
                        .Rule(new WhitespaceTerminal())))
                .GetGrammar();
            return new LexerRule(new NonTerminal("whitespace"), whitespaceGrammar);
        }

        private ILexerRule CreateWordRule()
        {
            var wordGrammar = new GrammarBuilder("W", p => p
                    .Production("W", r => r
                        .Rule("word")
                        .Rule("word", "W"))
                    .Production("word", r => r
                        .Rule(new RangeTerminal('a', 'z'))
                        .Rule(new RangeTerminal('A', 'Z'))
                        .Rule(new RangeTerminal('0', '9'))))
                .GetGrammar();
            return new LexerRule(new NonTerminal("word"), wordGrammar);
        }

        [TestMethod]
        public void Test_ParserRunner_That_Parses_Simple_Word_Sequence()
        {
            var input = "this is input";
            var parseRunner = new ParseRunner(_grammar, input);
            var count = 0;
            while (parseRunner.Pulse() && count < 100)
            {
                count++;
            }

            Assert.IsTrue(parseRunner.Parser.IsAccepted());
        }
    }
}
