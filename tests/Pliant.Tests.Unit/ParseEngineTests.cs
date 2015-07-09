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
            var grammar = new GrammarBuilder("S")
                .Production("S", r => r
                    .Rule("A", "T")
                    .Rule(a, "T"))
                .Production("A", r=>r
                    .Rule(a)
                    .Rule("B", "A"))
                .Production("B", r => r
                    .Lambda())
                .Production("T", r=>r
                    .Rule(b, b, b))
                .ToGrammar();
            var T_Production = grammar.Productions[3];

            var parseEngine = new ParseEngine(grammar);
            ParseInput(parseEngine, tokens);

            var S_0_4 = parseEngine.GetRoot() as ISymbolNode;
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
            var grammar = new GrammarBuilder("S")
                .Production("S", r => r
                    .Rule(a))
                .ToGrammar();

            var parseEngine = new ParseEngine(grammar);
            ParseInput(parseEngine, tokens);

            var parseNode = parseEngine.GetRoot();
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
            var grammar = new GrammarBuilder("S")
                .Production("S", r => r
                    .Rule("A"))
                .Production("A", r => r
                    .Rule(a))
                .ToGrammar();
            var parseEngine = new ParseEngine(grammar);
            ParseInput(parseEngine, tokens);

            /*  S_0_1 -> A_0_1
             *  A_0_1 -> 'a'
             */
            var S_0_1 = parseEngine.GetRoot() as ISymbolNode;
            Assert.IsNotNull(S_0_1);
            Assert.AreEqual(1, S_0_1.Children.Count);
            
            var S_0_1_1 = S_0_1.Children[0] as IAndNode;
            Assert.IsNotNull(S_0_1_1);
            Assert.AreEqual(1, S_0_1_1.Children.Count);

            var A_0_1 = S_0_1_1.Children[0] as ISymbolNode;
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
            var grammar = new GrammarBuilder("S")
                .Production("S", r => r
                    .Rule("A"))
                .Production("A", r => r
                    .Rule(a, "A")
                    .Rule(b))
                .ToGrammar();
            var tokens = Tokenize( "ab");
            var parseEngine = new ParseEngine(grammar);
            ParseInput(parseEngine, tokens);

            /*  S_0_2 -> A_0_2
             *  A_0_2 -> a_0_1 A_1_2
             *  A_1_2 -> b_1_2
             */
            var S_0_2 = parseEngine.GetRoot() as ISymbolNode;
            Assert.IsNotNull(S_0_2);
            Assert.AreEqual(1, S_0_2.Children.Count);
            
            var S_0_2_1 = S_0_2.Children[0] as IAndNode;
            Assert.IsNotNull(S_0_2_1);
            Assert.AreEqual(1, S_0_2_1.Children.Count);

            var A_0_2 = S_0_2_1.Children[0] as ISymbolNode;
            Assert.IsNotNull(A_0_2);
            Assert.AreEqual(1, A_0_2.Children.Count);

            var A_0_2_1 = A_0_2.Children[0] as IAndNode;
            Assert.IsNotNull(A_0_2_1);
            Assert.AreEqual(2, A_0_2_1.Children.Count);

            var a_0_1 = A_0_2_1.Children[0] as ITokenNode;
            Assert.IsNotNull(a_0_1);
            Assert.AreEqual("a", a_0_1.Token.Value);

            var A_1_2 = A_0_2_1.Children[1] as ISymbolNode;
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

            var grammar = new GrammarBuilder("S")
                .Production("S", r => r
                    .Rule("A"))
                .Production("A", r => r
                    .Rule(a, "B"))
                .Production("B", r => r
                    .Rule("A")
                    .Rule(b))
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
            var S_0_4 = parseEngine.GetRoot() as ISymbolNode;
            Assert.IsNotNull(S_0_4);
            Assert.AreEqual(1, S_0_4.Children.Count);

            var S_0_4_1 = S_0_4.Children[0] as IAndNode;
            Assert.IsNotNull(S_0_4_1);
            Assert.AreEqual(1, S_0_4_1.Children.Count);

            var A_0_4 = S_0_4_1.Children[0] as ISymbolNode;
            Assert.IsNotNull(A_0_4);
            Assert.AreEqual(1, A_0_4.Children.Count);

            var A_0_4_1 = A_0_4.Children[0] as IAndNode;
            Assert.IsNotNull(A_0_4_1);
            Assert.AreEqual(2, A_0_4_1.Children.Count);
            
            var a_0_1 = A_0_4_1.Children[0] as ITokenNode;
            Assert.IsNotNull(a_0_1);
            Assert.AreEqual("a", a_0_1.Token.Value);

            var B_1_4 = A_0_4_1.Children[1] as ISymbolNode;
            Assert.IsNotNull(B_1_4);
            Assert.AreEqual(1, B_1_4.Children.Count);

            var B_1_4_1 = B_1_4.Children[0] as IAndNode;
            Assert.IsNotNull(B_1_4_1);
            Assert.AreEqual(1, B_1_4_1.Children.Count);

            var A_1_4 = B_1_4_1.Children[0] as ISymbolNode;
            Assert.IsNotNull(A_1_4);
            Assert.AreEqual(1, A_1_4.Children.Count);

            var A_1_4_1 = A_1_4.Children[0] as IAndNode;
            Assert.IsNotNull(A_1_4_1);
            Assert.AreEqual(2, A_1_4_1.Children.Count);
            
            var a_1_2 = A_1_4_1.Children[0] as ITokenNode;
            Assert.IsNotNull(a_1_2);
            Assert.AreEqual("a", a_1_2.Token.Value);

            var B_2_4 = A_1_4_1.Children[1] as ISymbolNode;
            Assert.IsNotNull(B_2_4);
            Assert.AreEqual(1, B_2_4.Children.Count);

            var B_2_4_1 = B_2_4.Children[0] as IAndNode;
            Assert.IsNotNull(B_2_4_1);
            Assert.AreEqual(1, B_2_4_1.Children.Count);

            var A_2_4 = B_2_4_1.Children[0] as ISymbolNode;
            Assert.IsNotNull(A_2_4);
            Assert.AreEqual(1, A_2_4.Children.Count);

            var A_2_4_1 = A_2_4.Children[0] as IAndNode;
            Assert.IsNotNull(A_2_4_1);
            Assert.AreEqual(2, A_2_4_1.Children.Count);

            var a_2_3 = A_2_4_1.Children[0] as ITokenNode;
            Assert.IsNotNull(a_2_3);
            Assert.AreEqual("a", a_2_3.Token.Value);

            var B_3_4 = A_2_4_1.Children[1] as ISymbolNode;
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

            var grammar = new GrammarBuilder("S")
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
                    .Rule(star))
                .ToGrammar();

            var tokens = Tokenize(".+");
            var parseEngine = new ParseEngine(grammar);
            ParseInput(parseEngine, tokens);

            var parseForest = parseEngine.GetRoot();
            Assert.IsNotNull(parseForest);

            // S_0_2 -> A_0_2
            // A_0_2 -> B_0_1 C_1_2
            // B_0_1 -> '.'_0_1
            // C_1_2 -> '+'_1_2
            var S_0_2 = parseForest as ISymbolNode;
            Assert.IsNotNull(S_0_2);
            Assert.AreEqual(1, S_0_2.Children.Count);

            var S_0_2_1 = S_0_2.Children[0] as IAndNode;
            Assert.IsNotNull(S_0_2_1);
            Assert.AreEqual(1, S_0_2_1.Children.Count);

            var A_0_2 = S_0_2_1.Children[0] as ISymbolNode;
            Assert.IsNotNull(A_0_2);
            Assert.AreEqual(1, A_0_2.Children.Count);

            var A_0_2_1 = A_0_2.Children[0] as IAndNode;
            Assert.IsNotNull(A_0_2_1);
            Assert.AreEqual(2, A_0_2_1.Children.Count);

            var B_0_1 = A_0_2_1.Children[0] as ISymbolNode;
            Assert.IsNotNull(B_0_1);
            Assert.AreEqual(1, B_0_1.Children.Count);

            var B_0_1_1 = B_0_1.Children[0] as IAndNode;
            Assert.IsNotNull(B_0_1_1);
            Assert.AreEqual(1, B_0_1_1.Children.Count);

            var dot_0_1 = B_0_1_1.Children[0] as ITokenNode;
            Assert.IsNotNull(dot_0_1);
            Assert.AreEqual(".", dot_0_1.Token.Value);

            var C_1_2 = A_0_2_1.Children[1] as ISymbolNode;
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
            var grammar = new GrammarBuilder("A")
                .Production("A", r => r
                    .Rule("B", "C"))
                .Production("B", r => r
                    .Rule(b))
                .Production("C", r => r
                    .Rule(c))
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

            var grammar = new GrammarBuilder("S")
                .Production("S", r => r
                    .Rule(a, "S", a)
                    .Rule(a))
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

            var grammar = new GrammarBuilder("S")
                .Production("S", r => r
                    .Rule("S", "L")
                    .Lambda())
                .Production("L", r => r
                    .Rule(a_to_z, "L`"))
                .Production("L`", r => r
                    .Rule(a_to_z, "L`")
                    .Lambda())
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
            var grammar = new GrammarBuilder("A")
                .Production("A", r => r
                    .Rule(a, "A")
                    .Lambda())
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

            var grammar = new GrammarBuilder("S")
                .Production("S", r => r
                    .Rule("A"))
                .Production("A", r => r
                    .Rule(a, "B"))
                .Production("B", r => r
                    .Rule("A")
                    .Rule(b))
                .ToGrammar();
            var input = Tokenize("aaab");
            var parseEngine = new ParseEngine(grammar);
            ParseInput(parseEngine, input);
        }

        [TestMethod]
        public void Test_ParseEngine_That_Right_Recursive_To_Normal_Transition_Creates_Correct_Parse_Tree()
        {
            IGrammar grammar = CreateRegularExpressionStubGrammar();

            var input = Tokenize("aaaaaaa");
            var parseEngine = new ParseEngine(grammar);
            ParseInput(parseEngine, input);

            // R_0_7 -> E_0_7
            // E_0_7 -> T_0_7
            // T_0_7 -> F_0_1 T_1_7
            // F_0_1 -> 'a'
            // T_1_7 -> F_1_2 T_2_7
            // F_1_2 -> 'a'
            // T_2_7 -> F_2_3 T_3_7
            // F_2_3 -> 'a'
            // T_3_7 -> F_3_4 T_4_7
            // F_3_4 -> 'a'
            // T_4_7 -> F_4_5 T_5_7
            // F_4_5 -> 'a'
            // T_5_7 -> F_5_6 T_6_7
            // F_5_6 -> 'a'
            // T_6_7 -> F_6_7
            // F_6_7 -> 'a'
            var R_0_7 = CastAndCountChildren<ISymbolNode>(parseEngine.GetRoot(), 1);
            AssertNodeProperties(R_0_7, "R", 0, 7);
            var E_0_7 = GetAndCastChildAtIndex<ISymbolNode>(R_0_7, 0);
            AssertNodeProperties(E_0_7, "E", 0, 7);
            var T_0_7 = GetAndCastChildAtIndex<ISymbolNode>(E_0_7, 0);
            AssertNodeProperties(T_0_7, "T", 0, 7);
            var F_0_1 = GetAndCastChildAtIndex<ISymbolNode>(T_0_7, 0);
            AssertNodeProperties(F_0_1, "F", 0, 1);
            var T_1_7 = GetAndCastChildAtIndex<ISymbolNode>(T_0_7, 1);
            AssertNodeProperties(T_1_7, "T", 1, 7);
            var F_1_2 = GetAndCastChildAtIndex<ISymbolNode>(T_1_7, 0);
            AssertNodeProperties(F_1_2, "F", 1, 2);
            var T_2_7 = GetAndCastChildAtIndex<ISymbolNode>(T_1_7, 1);
            AssertNodeProperties(T_2_7, "T", 2, 7);
            var F_2_3 = GetAndCastChildAtIndex<ISymbolNode>(T_2_7, 0);
            AssertNodeProperties(F_2_3, "F", 2, 3);
            var T_3_7 = GetAndCastChildAtIndex<ISymbolNode>(T_2_7, 1);
            AssertNodeProperties(T_3_7, "T", 3, 7);
            var F_3_4 = GetAndCastChildAtIndex<ISymbolNode>(T_3_7, 0);
            AssertNodeProperties(F_3_4, "F", 3, 4);
            var T_4_7 = GetAndCastChildAtIndex<ISymbolNode>(T_3_7, 1);
            AssertNodeProperties(T_4_7, "T", 4, 7);
            var F_4_5 = GetAndCastChildAtIndex<ISymbolNode>(T_4_7, 0);
            AssertNodeProperties(F_4_5, "F", 4, 5);
            var T_5_7 = GetAndCastChildAtIndex<ISymbolNode>(T_4_7, 1);
            AssertNodeProperties(T_5_7, "T", 5, 7);
            var F_5_6 = GetAndCastChildAtIndex<ISymbolNode>(T_5_7, 0);
            AssertNodeProperties(F_5_6, "F", 5, 6);
            var T_6_7 = GetAndCastChildAtIndex<ISymbolNode>(T_5_7, 1);
            AssertNodeProperties(T_6_7, "T", 6, 7);
            var F_6_7 = GetAndCastChildAtIndex<ISymbolNode>(T_6_7, 0);
            AssertNodeProperties(F_6_7, "F", 6, 7);
        }

        [TestMethod]
        public void Test_ParseEngine_That_Creates_Correct_Parse_Tree_When_Multiple_Leo_Items_Exist_On_Search_Path()
        {
            var grammar = CreateRegularExpressionStubGrammar();
            var input = Tokenize("aaa");
            var parseEngine = new ParseEngine(grammar);
            ParseInput(parseEngine, input);

            var R_0_3 = CastAndCountChildren<ISymbolNode>(parseEngine.GetRoot(), 1);
            AssertNodeProperties(R_0_3, "R", 0, 3);
            var E_0_3 = GetAndCastChildAtIndex<ISymbolNode>(R_0_3, 0);
            AssertNodeProperties(E_0_3, "E", 0, 3);
            var T_0_3 = GetAndCastChildAtIndex<ISymbolNode>(E_0_3, 0);
            AssertNodeProperties(T_0_3, "T", 0, 3);
            var F_0_1 = GetAndCastChildAtIndex<ISymbolNode>(T_0_3, 0);
            AssertNodeProperties(F_0_1, "F", 0, 1);
            var A_0_1 = GetAndCastChildAtIndex<ISymbolNode>(F_0_1, 0);
            AssertNodeProperties(A_0_1, "A", 0, 1);
            var T_1_3 = GetAndCastChildAtIndex<ISymbolNode>(T_0_3, 1);
            AssertNodeProperties(T_1_3, "T", 1, 3);
            var F_1_2 = GetAndCastChildAtIndex<ISymbolNode>(T_1_3, 0);
            AssertNodeProperties(F_1_2, "F", 1, 2);
            var T_2_3 = GetAndCastChildAtIndex<ISymbolNode>(T_1_3, 1);
            AssertNodeProperties(T_2_3, "T", 2, 3);
            var F_2_3 = GetAndCastChildAtIndex<ISymbolNode>(T_2_3, 0);
            AssertNodeProperties(F_2_3, "F", 2, 3);
        }


        private static IGrammar CreateRegularExpressionStubGrammar()
        {
            return new GrammarBuilder("R")
                .Production("R", r => r
                    .Rule("E")
                    .Rule('^', "E")
                    .Rule("E", '$')
                    .Rule('^', "E", '$'))
                .Production("E", r => r
                    .Rule("T")
                    .Rule("T", '|', "E")
                    .Lambda())
                .Production("T", r => r
                    .Rule("F", "T")
                    .Rule("F"))
                .Production("F", r => r
                    .Rule("A")
                    .Rule("A", "I"))
                .Production("A", r=>r
                    .Rule('a'))
                .Production("I", r=>r
                    .Rule('+', '?', '*'))
                .ToGrammar();
        }

        private static void AssertNodeProperties(ISymbolNode node, string nodeName, int origin, int location)
        {
            Assert.AreEqual(nodeName, (node.Symbol as INonTerminal).Value);
            Assert.AreEqual(origin, node.Origin);
            Assert.AreEqual(location, node.Location);
        }

        private T CastAndCountChildren<T>(INode node, int childCount)
            where T : class, IInternalNode
        {
            var tNode = node as T;
            Assert.IsNotNull(node);            
            Assert.AreEqual(1, tNode.Children.Count);
            var firstAndNode = tNode.Children[0];
            Assert.IsNotNull(firstAndNode);
            Assert.AreEqual(childCount, firstAndNode.Children.Count);
            return tNode;
        }

        private T GetAndCastChildAtIndex<T>(IInternalNode node, int index)
            where T : class, INode
        {
            var firstAndNode = node.Children[0];
            Assert.IsNotNull(firstAndNode);
            Assert.IsFalse(index > firstAndNode.Children.Count);
            var child = firstAndNode.Children[index] as T;
            Assert.IsNotNull(child);
            return child;
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

            var expressionGrammar = new GrammarBuilder("S")
                .Production("S", r => r
                    .Rule("S", plus, "M")
                    .Rule("M"))
                .Production("M", r => r
                    .Rule("M", star, "T")
                    .Rule("T"))
                .Production("T", r => r
                    .Rule(digit))
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
