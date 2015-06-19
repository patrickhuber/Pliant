using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Nodes;
using Pliant.Builders;
using Pliant.Grammars;
using Pliant.Tokens;
using System.Linq;
using System.Collections.Generic;
using Pliant.Charts;

namespace Pliant.Tests.Unit
{
    [TestClass]
    public class ParseEngineTests
    {
        [TestMethod]
        public void Test_ParseEngine_That_Ambiguous_Grammar_Creates_Multiple_Parse_Paths()
        {
            // example 3 section 4, Elizabeth Scott
            var tokens = Tokenize("abbb");

            var a = new TerminalLexerRule(new Terminal('a'), new TokenType("a"));
            var b = new TerminalLexerRule(new Terminal('b'), new TokenType("b"));
            var grammar = new GrammarBuilder("S", p => p
                    .Production("S", r => r
                        .Rule("A", "T")
                        .Rule(a, "T"))
                    .Production("A", r=>r
                        .Rule(a)
                        .Rule("B", "A"))
                    .Production("B", r => r
                        .Lambda())
                    .Production("T", r=>r
                        .Rule(b, b, b)))
                .ToGrammar();
            var T_Production = grammar.Productions[3];

            var parseEngine = new ParseEngine(grammar);
            ParseInput(parseEngine, tokens);

            var S_0_4 = parseEngine.GetParseForest() as ISymbolNode;
            Assert.IsNotNull(S_0_4);
            Assert.AreEqual(2, S_0_4.Children.Count);

            var S_0_4_1 = S_0_4.Children[0] as IAndNode;
            Assert.IsNotNull(S_0_4_1);
            Assert.AreEqual(2, S_0_4_1.Children.Count);

            var a_0_1 = S_0_4_1.Children[0] as ITokenNode;
            Assert.IsNotNull(a_0_1);
            Assert.AreEqual("a", a_0_1.Token.Value);

            var T_1_4 = S_0_4_1.Children[1] as ISymbolNode;
            Assert.IsNotNull(T_1_4);
            Assert.AreEqual(1, T_1_4.Children.Count);
                
            var S_0_4_2 = S_0_4.Children[1] as IAndNode;
            Assert.IsNotNull(S_0_4_2);
            Assert.AreEqual(2, S_0_4_2.Children.Count);

            var A_0_1 = S_0_4_2.Children[0] as ISymbolNode;
            Assert.IsNotNull(A_0_1);
            Assert.AreEqual(2, A_0_1.Children.Count);

            var A_0_1_1 = A_0_1.Children[0] as IAndNode;
            Assert.IsNotNull(A_0_1_1);
            Assert.AreEqual(1, A_0_1_1.Children.Count);

            Assert.AreSame(a_0_1, A_0_1_1.Children[0]);

            var A_0_1_2 = A_0_1.Children[1] as IAndNode;
            Assert.IsNotNull(A_0_1_1);
            Assert.AreEqual(2, A_0_1_2.Children.Count);
            
            Assert.AreSame(A_0_1, A_0_1_2.Children[1]);

            var B_0_0 = A_0_1_2.Children[0] as ISymbolNode;
            Assert.IsNotNull(B_0_0);
            Assert.AreEqual(1, B_0_0.Children.Count);

            var B_0_0_1 = B_0_0.Children[0] as IAndNode;
            Assert.IsNotNull(B_0_0_1);
            Assert.AreEqual(1, B_0_0_1.Children.Count);

            var nullTerminal = B_0_0_1.Children[0] as ITerminalNode;
            Assert.IsNotNull(nullTerminal);
            Assert.AreEqual('\0', nullTerminal.Capture);

            var T_1_4_1 = T_1_4.Children[0] as IAndNode;
            Assert.IsNotNull(T_1_4_1);
            Assert.AreEqual(2, T_1_4_1.Children.Count);

            var T_1_3 = T_1_4_1.Children[0] as IIntermediateNode;
            Assert.IsNotNull(T_1_3);
            Assert.AreEqual(1, T_1_3.Children.Count);
            
            var b_3_4 = T_1_4_1.Children[1] as ITokenNode;
            Assert.IsNotNull(b_3_4);
            Assert.AreEqual("b", b_3_4.Token.Value);

            var T_1_3_1 = T_1_3.Children[0] as IAndNode;
            Assert.IsNotNull(T_1_3_1);
            Assert.AreEqual(2, T_1_3_1.Children.Count);

            var b_1_2 = T_1_3_1.Children[0] as ITokenNode;
            Assert.IsNotNull(b_1_2);
            Assert.AreEqual("b", b_1_2.Token.Value);

            var b_2_3 = T_1_3_1.Children[1] as ITokenNode;
            Assert.IsNotNull(b_2_3);
            Assert.AreEqual("b", b_2_3.Token.Value);
        }

        [TestMethod]
        public void Test_ParseEngine_That_Completed_Scan_Creates_Internal_And_Terminal_Node()
        {
            var tokens = Tokenize("a");
            var a = new TerminalLexerRule(new Terminal('a'), new TokenType("a"));
            var grammar = new GrammarBuilder("S", p => p
                    .Production("S", r => r
                        .Rule(a)))
                .ToGrammar();

            var parseEngine = new ParseEngine(grammar);
            ParseInput(parseEngine, tokens);

            var parseNode = parseEngine.GetParseForest();
            Assert.IsNotNull(parseNode);

            var S_0_1 = parseNode as ISymbolNode;
            Assert.IsNotNull(S_0_1);
            Assert.AreEqual(1, S_0_1.Children.Count);

            var S_0_1_1 = S_0_1.Children[0] as IAndNode;
            Assert.IsNotNull(S_0_1_1);
            Assert.AreEqual(1, S_0_1_1.Children.Count);

            var a_0_1 = S_0_1_1.Children[0] as ITokenNode;
            Assert.IsNotNull(a_0_1);
            Assert.AreEqual("a", a_0_1.Token.Value);
        }

        [TestMethod]
        public void Test_ParseEngine_That_Completed_Prediction_Creates_Internal_Node()
        {
            var tokens = Tokenize("a");
            var a = new TerminalLexerRule(new Terminal('a'), new TokenType("a"));
            var grammar = new GrammarBuilder("S", p => p
                    .Production("S", r => r
                        .Rule("A"))
                    .Production("A", r => r
                        .Rule(a)))
                .ToGrammar();
            var parseEngine = new ParseEngine(grammar);
            ParseInput(parseEngine, tokens);

            /*  S_0_1 -> A_0_1
             *  A_0_1 -> 'a'
             */
            var S_0_1 = parseEngine.GetParseForest() as IInternalNode;
            Assert.IsNotNull(S_0_1);
            Assert.AreEqual(1, S_0_1.Children.Count);
            
            var S_0_1_1 = S_0_1.Children[0] as IAndNode;
            Assert.IsNotNull(S_0_1_1);
            Assert.AreEqual(1, S_0_1_1.Children.Count);

            var A_0_1 = S_0_1_1.Children[0] as IInternalNode;
            Assert.IsNotNull(A_0_1);
            Assert.AreEqual(1, A_0_1.Children.Count);

            var A_0_1_1 = A_0_1.Children[0] as IAndNode;
            Assert.IsNotNull(A_0_1_1);
            Assert.AreEqual(1, A_0_1_1.Children.Count);

            var a_0_1 = A_0_1_1.Children[0] as ITokenNode;
            Assert.IsNotNull(a_0_1);
            Assert.AreEqual("a", a_0_1.Token.Value);
        }

        [TestMethod]
        public void Test_ParseEngine_That_Leo_Items_Generate_Proper_Parse_Tree()
        {
            var a = new TerminalLexerRule(new Terminal('a'), new TokenType("a"));
            var b = new TerminalLexerRule(new Terminal('b'), new TokenType("b"));
            var grammar = new GrammarBuilder("S", p => p
                    .Production("S", r => r
                        .Rule("A"))
                    .Production("A", r => r
                        .Rule(a, "A")
                        .Rule(b)))
                .ToGrammar();
            var tokens = Tokenize( "ab");
            var parseEngine = new ParseEngine(grammar);
            ParseInput(parseEngine, tokens);

            /*  S_0_2 -> A_0_2
             *  A_0_2 -> a_0_1 A_1_2
             *  A_1_2 -> b_1_2
             */
            var S_0_2 = parseEngine.GetParseForest() as IInternalNode;
            Assert.IsNotNull(S_0_2);
            Assert.AreEqual(1, S_0_2.Children.Count);
            
            var S_0_2_1 = S_0_2.Children[0] as IAndNode;
            Assert.IsNotNull(S_0_2_1);
            Assert.AreEqual(1, S_0_2_1.Children.Count);

            var A_0_2 = S_0_2_1.Children[0] as IInternalNode;
            Assert.IsNotNull(A_0_2);
            Assert.AreEqual(1, A_0_2.Children.Count);

            var A_0_2_1 = A_0_2.Children[0] as IAndNode;
            Assert.IsNotNull(A_0_2_1);
            Assert.AreEqual(2, A_0_2_1.Children.Count);

            var a_0_1 = A_0_2_1.Children[0] as ITokenNode;
            Assert.IsNotNull(a_0_1);
            Assert.AreEqual("a", a_0_1.Token.Value);

            var A_1_2 = A_0_2_1.Children[1] as IInternalNode;
            Assert.IsNotNull(A_1_2);
            Assert.AreEqual(1, A_1_2.Children.Count);

            var A_1_2_1 = A_1_2.Children[0] as IAndNode;
            Assert.IsNotNull(A_1_2_1);
            Assert.AreEqual(1, A_1_2_1.Children.Count);

            var b_1_2 = A_1_2_1.Children[0] as ITokenNode;
            Assert.IsNotNull(b_1_2);
            Assert.AreEqual("b", b_1_2.Token.Value);
        }


        [TestMethod]
        public void Test_ParseEngine_That_PassThrough_Recursive_Items_Creates_Virtual_Nodes()
        {
            var a = new TerminalLexerRule(new Terminal('a'), new TokenType("a"));
            var b = new TerminalLexerRule(new Terminal('b'), new TokenType("b"));

            var grammar = new GrammarBuilder("S", p => p
                .Production("S", r => r
                    .Rule("A"))
                .Production("A", r => r
                    .Rule(a, "B"))
                .Production("B", r => r
                    .Rule("A")
                    .Rule(b)))
            .ToGrammar();
            var tokens = Tokenize( "aaab");

            var parseEngine = new ParseEngine(grammar);
            ParseInput(parseEngine, tokens);

            /*  S_0_4 -> A_0_4
             *  A_0_4 -> 'a' B_1_4
             *  B_1_4 -> A_1_4
             *  A_1_4 -> 'a' B_2_4
             *  B_2_4 -> A_2_4
             *  A_2_4 -> 'a' B_3_4
             *  B_3_4 -> 'b'
             */
            var S_0_4 = parseEngine.GetParseForest() as IInternalNode;
            Assert.IsNotNull(S_0_4);
            Assert.AreEqual(1, S_0_4.Children.Count);

            var S_0_4_1 = S_0_4.Children[0] as IAndNode;
            Assert.IsNotNull(S_0_4_1);
            Assert.AreEqual(1, S_0_4_1.Children.Count);

            var A_0_4 = S_0_4_1.Children[0] as IInternalNode;
            Assert.IsNotNull(A_0_4);
            Assert.AreEqual(1, A_0_4.Children.Count);

            var A_0_4_1 = A_0_4.Children[0] as IAndNode;
            Assert.IsNotNull(A_0_4_1);
            Assert.AreEqual(2, A_0_4_1.Children.Count);
            
            var a_0_1 = A_0_4_1.Children[0] as ITokenNode;
            Assert.IsNotNull(a_0_1);
            Assert.AreEqual("a", a_0_1.Token.Value);

            var B_1_4 = A_0_4_1.Children[1] as IInternalNode;
            Assert.IsNotNull(B_1_4);
            Assert.AreEqual(1, B_1_4.Children.Count);

            var B_1_4_1 = B_1_4.Children[0] as IAndNode;
            Assert.IsNotNull(B_1_4_1);
            Assert.AreEqual(1, B_1_4_1.Children.Count);

            var A_1_4 = B_1_4_1.Children[0] as IInternalNode;
            Assert.IsNotNull(A_1_4);
            Assert.AreEqual(1, A_1_4.Children.Count);

            var A_1_4_1 = A_1_4.Children[0] as IAndNode;
            Assert.IsNotNull(A_1_4_1);
            Assert.AreEqual(2, A_1_4_1.Children.Count);
            
            var a_1_2 = A_1_4_1.Children[0] as ITokenNode;
            Assert.IsNotNull(a_1_2);
            Assert.AreEqual("a", a_1_2.Token.Value);

            var B_2_4 = A_1_4_1.Children[1] as IInternalNode;
            Assert.IsNotNull(B_2_4);
            Assert.AreEqual(1, B_2_4.Children.Count);

            var B_2_4_1 = B_2_4.Children[0] as IAndNode;
            Assert.IsNotNull(B_2_4_1);
            Assert.AreEqual(1, B_2_4_1.Children.Count);

            var A_2_4 = B_2_4_1.Children[0] as IInternalNode;
            Assert.IsNotNull(A_2_4);
            Assert.AreEqual(1, A_2_4.Children.Count);

            var A_2_4_1 = A_2_4.Children[0] as IAndNode;
            Assert.IsNotNull(A_2_4_1);
            Assert.AreEqual(2, A_2_4_1.Children.Count);

            var a_2_3 = A_2_4_1.Children[0] as ITokenNode;
            Assert.IsNotNull(a_2_3);
            Assert.AreEqual("a", a_2_3.Token.Value);

            var B_3_4 = A_2_4_1.Children[1] as IInternalNode;
            Assert.IsNotNull(B_3_4);
            Assert.AreEqual(1, B_3_4.Children.Count);

            var B_3_4_1 = B_3_4.Children[0] as IAndNode;
            Assert.IsNotNull(B_3_4_1);
            Assert.AreEqual(1, B_3_4_1.Children.Count);

            var b_3_4 = B_3_4_1.Children[0] as ITokenNode;
            Assert.IsNotNull(b_3_4);
            Assert.AreEqual("b", b_3_4.Token.Value);
        }

        [TestMethod]
        public void Test_ParseEngine_That_Mid_Grammar_Right_Recursion_Handles_Null_Root_Transition_Item()
        {
            var dot = new TerminalLexerRule(new Terminal('.'), new TokenType("."));
            var plus = new TerminalLexerRule(new Terminal('+'), new TokenType("+"));
            var question = new TerminalLexerRule(new Terminal('?'), new TokenType("?"));
            var star = new TerminalLexerRule(new Terminal('*'), new TokenType("*"));

            var grammar = new GrammarBuilder("S", p => p
                    .Production("S", r => r
                        .Rule("A")
                        .Rule("A", "S"))
                    .Production("A", r => r
                        .Rule("B")
                        .Rule("B", "C"))
                    .Production("B", r => r
                        .Rule(dot))
                    .Production("C", r => r
                        .Rule(plus)
                        .Rule(question)
                        .Rule(star)))
                .ToGrammar();

            var tokens = Tokenize(".+");
            var parseEngine = new ParseEngine(grammar);
            ParseInput(parseEngine, tokens);

            var parseForest = parseEngine.GetParseForest();
            Assert.IsNotNull(parseForest);

            // S_0_2 -> A_0_2
            // A_0_2 -> B_0_1 C_1_2
            // B_0_1 -> '.'_0_1
            // C_1_2 -> '+'_1_2
            var S_0_2 = parseForest as IInternalNode;
            Assert.IsNotNull(S_0_2);
            Assert.AreEqual(1, S_0_2.Children.Count);

            var S_0_2_1 = S_0_2.Children[0] as IAndNode;
            Assert.IsNotNull(S_0_2_1);
            Assert.AreEqual(1, S_0_2_1.Children.Count);

            var A_0_2 = S_0_2_1.Children[0] as IInternalNode;
            Assert.IsNotNull(A_0_2);
            Assert.AreEqual(1, A_0_2.Children.Count);

            var A_0_2_1 = A_0_2.Children[0] as IAndNode;
            Assert.IsNotNull(A_0_2_1);
            Assert.AreEqual(2, A_0_2_1.Children.Count);

            var B_0_1 = A_0_2_1.Children[0] as IInternalNode;
            Assert.IsNotNull(B_0_1);
            Assert.AreEqual(1, B_0_1.Children.Count);

            var B_0_1_1 = B_0_1.Children[0] as IAndNode;
            Assert.IsNotNull(B_0_1_1);
            Assert.AreEqual(1, B_0_1_1.Children.Count);

            var dot_0_1 = B_0_1_1.Children[0] as ITokenNode;
            Assert.IsNotNull(dot_0_1);
            Assert.AreEqual(".", dot_0_1.Token.Value);

            var C_1_2 = A_0_2_1.Children[1] as IInternalNode;
            Assert.IsNotNull(C_1_2);

            var C_1_2_1 = C_1_2.Children[0] as IAndNode;
            Assert.IsNotNull(C_1_2_1);
            Assert.AreEqual(1, C_1_2_1.Children.Count);

            var plus_1_2 = C_1_2_1.Children[0] as ITokenNode;
            Assert.IsNotNull(plus_1_2);
            Assert.AreEqual("+", plus_1_2.Token.Value);
        }

        [TestMethod]
        public void Test_ParseEngine_That_Simple_Substitution_Grammar_Parses()
        {
            var b = new TerminalLexerRule(new Terminal('b'), new TokenType("b"));
            var c = new TerminalLexerRule(new Terminal('c'), new TokenType("c"));
            var grammar = new GrammarBuilder("A", p => p
                .Production("A", r => r
                    .Rule("B", "C"))
                .Production("B", r => r
                    .Rule(b))
                .Production("C", r => r
                    .Rule(c)))
            .ToGrammar();
            var tokens = Tokenize("bc");
            var parseEngine = new ParseEngine(grammar);
            ParseInput(parseEngine, tokens);
        }

        [TestMethod]
        public void Test_ParseEngine_That_Expression_Grammar_Parses_Expression()
        {
            var expressionGrammar = CreateExpressionGrammar();

            var tokens = new[]
            {
                CreateDigitToken(2, 0),
                CreateCharacterToken('+', 1),
                CreateDigitToken(3, 2),
                CreateCharacterToken('*', 3),
                CreateDigitToken(4, 4)
            };
            var parseEngine = new ParseEngine(expressionGrammar);
            ParseInput(parseEngine, tokens);
        }

        [TestMethod]
        public void Test_ParseEngine_That_Invalid_Input_Exists_Parse()
        {
            var grammar = CreateExpressionGrammar();
            var tokens = new[]
            {
                CreateDigitToken(1, 0),
                CreateCharacterToken('+', 1),
                CreateCharacterToken('b', 2), 
                CreateCharacterToken('*', 3),
                CreateDigitToken(3, 4)
            };
            var parseEngine = new ParseEngine(grammar);
            Assert.IsTrue(parseEngine.Pulse(tokens[0]));
            Assert.IsTrue(parseEngine.Pulse(tokens[1]));
            Assert.IsFalse(parseEngine.Pulse(tokens[2]));
        }

        [TestMethod]
        public void Test_ParseEngine_That_Unmarked_Middle_Recursion_Parses()
        {
            var a = new TerminalLexerRule(new Terminal('a'), new TokenType("a"));

            var grammar = new GrammarBuilder("S", p => p
                    .Production("S", r => r
                        .Rule(a, "S", a)
                        .Rule(a)))
                .ToGrammar();

            var parseEngine = new ParseEngine(grammar);

            var tokens = Tokenize("aaaaaaaaa");
            ParseInput(parseEngine, tokens);
        }

        [TestMethod]
        public void Test_ParseEngine_That_Right_Recursive_Quasi_Complete_Items_Are_Leo_Optimized()
        {
            var a_to_z = new TerminalLexerRule(
                new RangeTerminal('a', 'z'),
                new TokenType("range"));

            var grammar = new GrammarBuilder("S", p => p
                    .Production("S", r => r
                        .Rule("S", "L")
                        .Lambda())
                    .Production("L", r => r
                        .Rule(a_to_z, "L`"))
                    .Production("L`", r => r
                        .Rule(a_to_z, "L`")
                        .Lambda()))
                .ToGrammar();

            var input = Tokenize("thisisonelonginputstring");
            var parseEngine = new ParseEngine(grammar);
            ParseInput(parseEngine, input);
            Chart chart = GetChartFromParseEngine(parseEngine);
            // when this count is < 10 we know that quasi complete items are being processed successfully
            Assert.IsTrue(chart.EarleySets[23].Completions.Count < 10);
        }


        [TestMethod]
        public void Test_ParseEngine_That_Right_Recursion_Is_Not_O_N_3()
        {
            var a = new TerminalLexerRule(
                new Terminal('a'),
                new TokenType("a"));
            var grammar = new GrammarBuilder("A", p => p
                .Production("A", r => r
                    .Rule(a, "A")
                    .Lambda()))
            .ToGrammar();

            var input = Tokenize("aaaaa");
            var recognizer = new ParseEngine(grammar);
            ParseInput(recognizer, input);

            var chart = GetChartFromParseEngine(recognizer);
            // -- 0 --
            // A ->.a A		    (0)	 # Start
            // A ->.			(0)	 # Start
            //
            // ...
            // -- n --
            // n	A -> a.A		(n-1)	 # Scan a
            // n	A ->.a A		(n)	 # Predict
            // n	A ->.			(n)	 # Predict
            // n	A -> a A.		(n)	 # Predict
            // n	A : A -> a A.	(0)	 # Transition
            // n	A -> a A.		(0)	 # Complete
            Assert.AreEqual(input.Count() + 1, chart.Count);
            var lastEarleySet = chart.EarleySets[chart.EarleySets.Count - 1];
            Assert.AreEqual(3, lastEarleySet.Completions.Count);
            Assert.AreEqual(1, lastEarleySet.Transitions.Count);
            Assert.AreEqual(1, lastEarleySet.Predictions.Count);
            Assert.AreEqual(1, lastEarleySet.Scans.Count);
        }
        
        [TestMethod]
        public void Test_ParseEngine_That_Intermediate_Step_Creates_Transition_Items()
        {
            var a = new TerminalLexerRule(new Terminal('a'), new TokenType("a"));
            var b = new TerminalLexerRule(new Terminal('b'), new TokenType("b"));

            var grammar = new GrammarBuilder("S", p => p
                .Production("S", r => r
                    .Rule("A"))
                .Production("A", r => r
                    .Rule(a, "B"))
                .Production("B", r => r
                    .Rule("A")
                    .Rule(b)))
            .ToGrammar();
            var input = Tokenize("aaab");
            var parseEngine = new ParseEngine(grammar);
            ParseInput(parseEngine, input);
        }

        private static Chart GetChartFromParseEngine(ParseEngine parseEngine)
        {
            return new PrivateObject(parseEngine).GetField("_chart") as Chart;
        }

        private static IGrammar CreateExpressionGrammar()
        {
            var plus = new TerminalLexerRule(
                            new Terminal('+'),
                            new TokenType("+"));
            var star = new TerminalLexerRule(
                new Terminal('*'),
                new TokenType("*"));
            var digit = new TerminalLexerRule(
                new DigitTerminal(),
                new TokenType("digit"));

            var expressionGrammar = new GrammarBuilder(
                "S", p => p
                .Production("S", r => r
                    .Rule("S", plus, "M")
                    .Rule("M"))
                .Production("M", r => r
                    .Rule("M", star, "T")
                    .Rule("T"))
                .Production("T", r => r
                    .Rule(digit)))
            .ToGrammar();
            return expressionGrammar;
        }

        private IToken CreateDigitToken(int value, int position)
        {
            return new Token(value.ToString(), position, new TokenType("digit"));
        }

        private IToken CreateCharacterToken(char character, int position)
        {
            return new Token(character.ToString(), position, new TokenType(character.ToString()));
        }

        private IEnumerable<IToken> Tokenize(string input)
        {
            return input.Select((x, i) =>
                new Token(x.ToString(), i, new TokenType(x.ToString())));
        }

        private IEnumerable<IToken> Tokenize(string input, string tokenType)
        {
            return input.Select((x, i) =>
                new Token(x.ToString(), i, new TokenType(tokenType)));
        }

        private void ParseInput(IParseEngine parseEngine, IEnumerable<IToken> tokens)
        {
            foreach (var token in tokens)
                Assert.IsTrue(parseEngine.Pulse(token));
            Assert.IsTrue(parseEngine.IsAccepted());
        }
    }
}
