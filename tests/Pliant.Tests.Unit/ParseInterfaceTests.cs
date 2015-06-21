using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Builders;
using Pliant.Charts;
using Pliant.Grammars;
using Pliant.Tokens;
using System;

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
            var whitespaceGrammar = new GrammarBuilder("S")
                .Production("S", r => r
                    .Rule("whitespace")
                    .Rule("whitespace", "S"))
                .Production("whitespace", r=>r
                    .Rule(new WhitespaceTerminal()))
                .ToGrammar();
            return new GrammarLexerRule("whitespace", whitespaceGrammar);
        }

        private static IGrammarLexerRule CreateWordRule()
        {
            var wordGrammar = new GrammarBuilder("W")
                .Production("W", r => r
                    .Rule("word")
                    .Rule("word", "W"))
                .Production("word", r => r
                    .Rule(new RangeTerminal('a', 'z'))
                    .Rule(new RangeTerminal('A', 'Z'))
                    .Rule(new RangeTerminal('0', '9')))
                .ToGrammar();
            return new GrammarLexerRule("word", wordGrammar);
        }

        [TestMethod]
        public void Test_ParserInterface_That_Parses_Simple_Word_Sequence()
        {
            var grammar = new GrammarBuilder("S")
                .Production("S", r => r
                    .Rule(_whitespaceRule)
                    .Rule(_whitespaceRule, "S")
                    .Rule(_wordRule)
                    .Rule(_wordRule, "S"))
                .ToGrammar();

            var input = "this is input";
            var parseEngine = new ParseEngine(grammar);
            RunParse(parseEngine, input);
        }

        [TestMethod]
        public void Test_ParseInterface_That_Whitespace_Is_Ignored()
        {
            // a <word boundary> abc <word boundary> a <word boundary> a
            const string input = "a abc a a";
            var grammar = new GrammarBuilder("A")
                .Production("A", r => r
                    .Rule(_wordRule, "A")
                    .Rule(_wordRule))
                .LexerRule(_whitespaceRule)
                .LexerRule(_wordRule)
                .Ignore(_whitespaceRule)
                .ToGrammar();

            var parseEngine = new ParseEngine(grammar);
            RunParse(parseEngine, input);
        }


        [TestMethod]
        public void Test_ParseInterface_That_Emits_Token_Between_Lexer_Rules_And_Eof()
        {
            const string input = "aa";
            var a = new TerminalLexerRule('a');
            var grammar = new GrammarBuilder("S")
                .Production("S", r => r
                    .Rule(a, "S")
                    .Rule(a))
                .LexerRule(a)
                .ToGrammar();
            var parseEngine = new ParseEngine(grammar);
            var parseInterface = new ParseInterface(parseEngine, input);

            var chart = GetParseEngineChart(parseEngine);
            Assert.IsTrue(parseInterface.Read());
            Assert.AreEqual(1, chart.EarleySets.Count);
            Assert.IsTrue(parseInterface.Read());
            Assert.AreEqual(3, chart.EarleySets.Count);
        }

        [TestMethod]
        public void Test_ParseInterface_Given_Existing_Lexemes_When_Character_Matches_Then_It_Is_Added()
        {
            const string input = "aaaa";
            var aGrammar = new GrammarBuilder("A")
                .Production("A", r => r
                    .Rule('a', 'a'))
                .ToGrammar();
            var a = new GrammarLexerRule("a", aGrammar);
            var grammar = new GrammarBuilder("S")
                .Production("S", r => r
                    .Rule(a, "S")
                    .Rule(a))
                .LexerRule(a)
                .ToGrammar();
            var parseEngine = new ParseEngine(grammar);
            var parseInterface = new ParseInterface(parseEngine, input);

            var chart = GetParseEngineChart(parseEngine);
            Assert.IsTrue(parseInterface.Read());
            Assert.AreEqual(1, chart.EarleySets.Count);
            Assert.IsTrue(parseInterface.Read());
            Assert.AreEqual(1, chart.EarleySets.Count);
        }


        [TestMethod]
        public void Test_ParseInterface_Given_No_Existing_Lexemes_When_Character_Matches_Then_It_Is_Added_To_New_Lexeme()
        {
            const string input = "aaaa";
            var aGrammar = new GrammarBuilder("A")
                .Production("A", r => r
                    .Rule('a', 'a'))
                .ToGrammar();
            var a = new GrammarLexerRule("a", aGrammar);
            var grammar = new GrammarBuilder("S")
                .Production("S", r => r
                    .Rule(a, "S")
                    .Rule(a))
                .LexerRule(a)
                .ToGrammar();
            var parseEngine = new ParseEngine(grammar);
            var parseInterface = new ParseInterface(parseEngine, input);

            var chart = GetParseEngineChart(parseEngine);
            for(int i=0;i<3;i++)
                Assert.IsTrue(parseInterface.Read());
            Assert.AreEqual(2, chart.EarleySets.Count);
        }

        [TestMethod]
        public void Test_ParseInterface_When_Character_Should_Be_Ignored_Then_Emits_Token()
        {
            const string input = "aa aa";
            var grammar = new GrammarBuilder("S")
                .Production("S", r => r
                    .Rule(_wordRule, "S")
                    .Rule(_wordRule))
                .LexerRule(_whitespaceRule)
                .LexerRule(_wordRule)
                .Ignore(_whitespaceRule)
                .ToGrammar();

            var parseEngine = new ParseEngine(grammar);
            var parseInterface = new ParseInterface(parseEngine, input);
            Chart chart = GetParseEngineChart(parseEngine);
            for (int i = 0; i < 2; i++)
                Assert.IsTrue(parseInterface.Read());
            Assert.IsTrue(parseInterface.Read());
            Assert.AreEqual(2, chart.EarleySets.Count);
        }
                
        [TestMethod]
        public void Test_ParseInterface_When_Character_Matches_Next_Production_Then_Emits_Token()
        {
            const string input = "aabb";
            var aGrammar = new GrammarBuilder("A")
                .Production("A", r => r
                    .Rule('a', "A")
                    .Rule('a'))
                .ToGrammar();
            var a = new GrammarLexerRule("a", aGrammar);

            var bGrammar = new GrammarBuilder("B")
                .Production("B", r => r
                    .Rule('b', "B")
                    .Rule('b'))
                .ToGrammar();
            var b = new GrammarLexerRule("b", bGrammar);

            var grammar = new GrammarBuilder("S")
                .Production("S", r => r
                    .Rule(a, b))
                .ToGrammar();

            var parseEngine = new ParseEngine(grammar);
            var chart = GetParseEngineChart(parseEngine);
            var parseInterface = new ParseInterface(parseEngine, input);
            for (int i = 0; i < input.Length; i++)
            {
                Assert.IsTrue(parseInterface.Read());
                if (i < 2)
                    Assert.AreEqual(1, chart.Count);
                else if (i < 3)
                    Assert.AreEqual(2, chart.Count);
                else
                    Assert.AreEqual(3, chart.Count);
            }
        }

        [TestMethod]
        public void Test_ParseInterface_Given_Ignore_Characters_When_Overlap_With_Terminal_Then_Chooses_Terminal()
        {
            var input = "word \t\r\n word";
            
            var endOfLine = new StringLiteralLexerRule(
                Environment.NewLine, 
                new TokenType("EOL"));

            var grammar = new GrammarBuilder("S")
                .Production("S", r => r
                    .Rule(_wordRule, endOfLine, _wordRule))
                .Ignore(_whitespaceRule)
                .ToGrammar();

            var parseEngine = new ParseEngine(grammar);
            RunParse(parseEngine, input);
        }

        private static Chart GetParseEngineChart(ParseEngine parseEngine)
        {
            return new PrivateObject(parseEngine).GetField("_chart") as Chart;
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
