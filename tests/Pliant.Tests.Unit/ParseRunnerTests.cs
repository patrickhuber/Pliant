using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Builders;
using Pliant.Grammars;

namespace Pliant.Tests.Unit
{
    [TestClass]
    public class ParseRunnerTests
    {
        ILexerRule _whitespaceRule;
        ILexerRule _wordRule;

        public ParseRunnerTests()
        {
            _whitespaceRule = CreateWhitespaceRule();
            _wordRule = CreateWordRule();            
        }
        
        private static ILexerRule CreateWhitespaceRule()
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

        private static ILexerRule CreateWordRule()
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
            var grammar = new GrammarBuilder("S", p => p
                    .Production("S", r => r
                        .Rule(_whitespaceRule)
                        .Rule(_whitespaceRule, "S")
                        .Rule(_wordRule)
                        .Rule(_wordRule, "S")))
                .GetGrammar();

            var input = "this is input";
            var parseRunner = new ParseRunner(grammar, input);
            var count = 0;
            while (parseRunner.Pulse() && count < 100)
            {
                count++;
            }

            Assert.IsTrue(parseRunner.Parser.IsAccepted());
        }

        [TestMethod]
        public void Test_ParseRunner_That_Whitespace_Is_Ignored()
        {
            // a <word boundary> abc <word boundary> a <word boundary> a
            const string input = "a abc a a";
            var grammar = new GrammarBuilder("A", p => p
                    .Production("A", r => r
                        .Rule(_wordRule, "A")
                        .Rule(_wordRule)), l => l
                    .LexerRule(_whitespaceRule)
                    .LexerRule(_wordRule), action => action
                    .Ignore(_whitespaceRule.TokenType.Id))
                .GetGrammar();
            var parseRunner = new ParseRunner(grammar, input);
            int count = 0;

            while (parseRunner.Pulse() && count < 100) // look for runaway
            { count++; }
            Assert.IsTrue(parseRunner.Parser.IsAccepted());
        }
    }
}
