﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Builders;
using Pliant.Charts;
using Pliant.Grammars;

namespace Pliant.Tests.Unit
{
    [TestClass]
    public class ParseInterfaceTests
    {
        IGrammarLexerRule _whitespaceRule;
        IGrammarLexerRule _wordRule;

        public ParseInterfaceTests()
        {
            _whitespaceRule = CreateWhitespaceRule();
            _wordRule = CreateWordRule();            
        }
        
        private static IGrammarLexerRule CreateWhitespaceRule()
        {
            var whitespaceGrammar = new GrammarBuilder("S", p => p
                    .Production("S", r => r
                        .Rule("whitespace")
                        .Rule("whitespace", "S"))
                    .Production("whitespace", r=>r
                        .Rule(new WhitespaceTerminal())))
                .GetGrammar();
            return new GrammarLexerRule("whitespace", whitespaceGrammar);
        }

        private static IGrammarLexerRule CreateWordRule()
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
            return new GrammarLexerRule("word", wordGrammar);
        }

        [TestMethod]
        public void Test_ParserInterface_That_Parses_Simple_Word_Sequence()
        {
            var grammar = new GrammarBuilder("S", p => p
                    .Production("S", r => r
                        .Rule(_whitespaceRule)
                        .Rule(_whitespaceRule, "S")
                        .Rule(_wordRule)
                        .Rule(_wordRule, "S")))
                .GetGrammar();

            var input = "this is input";
            var parseEngine = new ParseEngine(grammar);
            RunParse(parseEngine, input);
        }

        [TestMethod]
        public void Test_ParseInterface_That_Whitespace_Is_Ignored()
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

            var parseEngine = new ParseEngine(grammar);
            RunParse(parseEngine, input);
        }


        [TestMethod]
        public void Test_ParseInterface_That_Emits_Token_Between_Lexer_Rules_And_Eof()
        {
            const string input = "aa";
            var a = new TerminalLexerRule('a');
            var grammar = new GrammarBuilder("S", p => p
                    .Production("S", r => r
                        .Rule(a, "S")
                        .Rule(a)), l => l
                    .LexerRule(a))
                .GetGrammar();
            var parseEngine = new ParseEngine(grammar);
            var parseInterface = new ParseInterface(parseEngine, input);

            var chart = new PrivateObject(parseEngine).GetField("_chart") as Chart;
            Assert.IsTrue(parseInterface.Read());
            Assert.AreEqual(1, chart.EarleySets.Count);
            Assert.IsTrue(parseInterface.Read());
            Assert.AreEqual(3, chart.EarleySets.Count);
        }

        [TestMethod]
        public void Test_ParseInterface_Given_Existing_Lexemes_When_Character_Matches_Then_It_Is_Added()
        {
            const string input = "aaaa";
            var aGrammar = new GrammarBuilder("A", p => p
                    .Production("A", r => r
                        .Rule('a', 'a')))
                .GetGrammar();
            var a = new GrammarLexerRule("a", aGrammar);
            var grammar = new GrammarBuilder("S", p => p
                    .Production("S", r => r
                        .Rule(a, "S")
                        .Rule(a)), l=>l
                    .LexerRule(a))
                .GetGrammar();
            var parseEngine = new ParseEngine(grammar);
            var parseInterface = new ParseInterface(parseEngine, input);

            var chart = new PrivateObject(parseEngine).GetField("_chart") as Chart;
            Assert.IsTrue(parseInterface.Read());
            Assert.AreEqual(1, chart.EarleySets.Count);
            Assert.IsTrue(parseInterface.Read());
            Assert.AreEqual(1, chart.EarleySets.Count);
        }


        [TestMethod]
        public void Test_ParseInterface_Given_No_Existing_Lexemes_When_Character_Matches_Then_It_Is_Added_To_New_Lexeme()
        {
            const string input = "aaaa";
            var aGrammar = new GrammarBuilder("A", p => p
                    .Production("A", r => r
                        .Rule('a', 'a')))
                .GetGrammar();
            var a = new GrammarLexerRule("a", aGrammar);
            var grammar = new GrammarBuilder("S", p => p
                    .Production("S", r => r
                        .Rule(a, "S")
                        .Rule(a)), l => l
                    .LexerRule(a))
                .GetGrammar();
            var parseEngine = new ParseEngine(grammar);
            var parseInterface = new ParseInterface(parseEngine, input);

            var chart = new PrivateObject(parseEngine).GetField("_chart") as Chart;
            for(int i=0;i<3;i++)
                Assert.IsTrue(parseInterface.Read());
            Assert.AreEqual(2, chart.EarleySets.Count);
        }

        [TestMethod]
        public void Test_ParseInterface_When_Character_Should_Be_Ignored_Then_Emits_Token()
        {
            const string input = "aa aa";
            var grammar = new GrammarBuilder("S", p => p
                    .Production("S", r => r
                        .Rule(_wordRule, "S")
                        .Rule(_wordRule)), l=>l
                    .LexerRule(_whitespaceRule)
                    .LexerRule(_wordRule), i=>i
                    .Ignore(_whitespaceRule.TokenType.Id))
                .GetGrammar();

            var parseEngine = new ParseEngine(grammar);
            var parseInterface = new ParseInterface(parseEngine, input);

            var chart = new PrivateObject(parseEngine).GetField("_chart") as Chart;
            for (int i = 0; i < 2; i++)
                Assert.IsTrue(parseInterface.Read());
            Assert.IsTrue(parseInterface.Read());
            Assert.AreEqual(2, chart.EarleySets.Count);
        }

        [TestMethod]
        public void Test_ParseInterface_When_Character_Is_Not_Matched_And_Not_Ignored_Then_Fails_Parse()
        {
            Assert.Fail();
        }


        private static void RunParse(ParseEngine parseEngine, string input)
        {
            var parseInterface = new ParseInterface(parseEngine, input);
            for (int i = 0; i < input.Length; i++)
                Assert.IsTrue(parseInterface.Read(), string.Format("Error parsing at position {0}", i));
            Assert.IsTrue(parseInterface.ParseEngine.IsAccepted());
        }

    }
}
