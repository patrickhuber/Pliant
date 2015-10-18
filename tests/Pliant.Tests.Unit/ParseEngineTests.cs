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

            ProductionBuilder B = "B", S = "S", T = "T", A = "A";
            
            S.Definition = (_) A + T | 'a' + T;
            A.Definition = (_) 'a' | B + A;
            B.Definition = null;
            T.Definition = (_) 'b' + 'b' + 'b';

            var grammar = new GrammarBuilder(S, new[] { S, A, B, T })
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
            ProductionBuilder S = "S";
            S.Definition = (_)'a';

            var grammar = new GrammarBuilder(S, new[] { S })
                .ToGrammar();

            var tokens = Tokenize("a");
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
            ProductionBuilder S = "S", A = "A";
            S.Definition = (_) A;
            A.Definition = (_) 'a';

            var grammar = new GrammarBuilder(S, new[] { S, A }).ToGrammar();

            var tokens = Tokenize("a");
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
            ProductionBuilder S = "S", A = "A";

            S.Definition = A;
            A.Definition = 'a' + A | 'b';

            var grammar = new GrammarBuilder(S, new[] { S, A }).ToGrammar();

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
            ProductionBuilder S = "S", A = "A", B = "B";
            S.Definition = A;
            A.Definition = 'a' + B;
            B.Definition = A | 'b';

            var grammar = new GrammarBuilder(S, new[] { S, A, B }).ToGrammar();
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
            ProductionBuilder S = "S", A = "A", B = "B", C = "C";
            S.Definition = (_) A | A + S;
            A.Definition = (_) B | B + C;
            B.Definition = (_) '.';
            C.Definition = (_) '+' | '?' | '*';

            var grammar = new GrammarBuilder(S, new[] { S, A, B, C }).ToGrammar();
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
            ProductionBuilder A = "A", B = "B", C = "C";
            A.Definition = (_) B + C;
            B.Definition = (_) 'b';
            C.Definition = (_) 'c';

            var grammar = new GrammarBuilder(A, new[] { A, B, C }).ToGrammar();
            var parseEngine = new ParseEngine(grammar);

            var tokens = Tokenize("bc");
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
            ProductionBuilder S = "S";
            S.Definition = 'a' + S + 'a' | 'a';

            var grammar = new GrammarBuilder(S, new[] { S }).ToGrammar();
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

            ProductionBuilder S = "S", L = "L", LP = "L`";
            S.Definition = (_)
                S + L 
                | (_) null;
            L.Definition = (_)
                a_to_z + LP;
            LP.Definition = (_)
                a_to_z + LP 
                | (_)null;
            var grammar = new GrammarBuilder(S, new[] {S, L, LP }).ToGrammar();
            var input = Tokenize("thisisonelonginputstring", "range");
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
                new CharacterTerminal('a'),
                new TokenType("a"));
            ProductionBuilder A = "A";
            A.Definition = 
                'a' + A 
                | (_)null;

            var grammar = new GrammarBuilder(A, new[] { A })
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
            ProductionBuilder S = "S", A = "A", B = "B";
            S.Definition = A;
            A.Definition = 'a' + B;
            B.Definition = A | 'b';
            var grammar = new GrammarBuilder(S, new[] { S, A, B }).ToGrammar();
            var input = Tokenize("aaab");
            var parseEngine = new ParseEngine(grammar);
            ParseInput(parseEngine, input);
        }

        [TestMethod]
        public void Test_ParseEngine_That_Right_Recursive_To_Normal_Transition_Creates_Correct_Parse_Tree()
        {
            IGrammar grammar = CreateRegularExpressionStubGrammar();

            var input = Tokenize("aaaa");
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
            var R_0_4 = CastAndCountChildren<ISymbolNode>(parseEngine.GetRoot(), 1);
            AssertNodeProperties(R_0_4, "R", 0, 4);
            var E_0_4 = GetAndCastChildAtIndex<ISymbolNode>(R_0_4, 0);
            AssertNodeProperties(E_0_4, "E", 0, 4);
            var T_0_4 = GetAndCastChildAtIndex<ISymbolNode>(E_0_4, 0);
            AssertNodeProperties(T_0_4, "T", 0, 4);
            var F_0_1 = GetAndCastChildAtIndex<ISymbolNode>(T_0_4, 0);
            AssertNodeProperties(F_0_1, "F", 0, 1);
            var T_1_4 = GetAndCastChildAtIndex<ISymbolNode>(T_0_4, 1);
            AssertNodeProperties(T_1_4, "T", 1, 4);
            var F_1_2 = GetAndCastChildAtIndex<ISymbolNode>(T_1_4, 0);
            AssertNodeProperties(F_1_2, "F", 1, 2);
            var T_2_4 = GetAndCastChildAtIndex<ISymbolNode>(T_1_4, 1);
            AssertNodeProperties(T_2_4, "T", 2, 4);
            var F_2_4 = GetAndCastChildAtIndex<ISymbolNode>(T_2_4, 0);
            AssertNodeProperties(F_2_4, "F", 2, 3);
            var T_3_4 = GetAndCastChildAtIndex<ISymbolNode>(T_2_4, 1);
            AssertNodeProperties(T_3_4, "T", 3, 4);
            var F_3_4 = GetAndCastChildAtIndex<ISymbolNode>(T_3_4, 0);
            AssertNodeProperties(F_3_4, "F", 3, 4);
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

        [TestMethod]
        public void Test_ParseEngine_That_Long_Production_Rule_Produces_Proper_Parse_Tree()
        {
            ProductionBuilder S = "S", A = "A", B = "B", C = "C", D = "D";
            S.Definition = (_)
                A + B + C + D + S
                | '|';
            A.Definition = 'a';
            B.Definition = 'b';
            C.Definition = 'c';
            D.Definition = 'd';
            var grammar = new GrammarBuilder(S, new[] { S, A, B, C, D }).ToGrammar();

            var input = Tokenize("abcdabcdabcdabcd|");
            var parseEngine = new ParseEngine(grammar);
            ParseInput(parseEngine, input);
            var root = parseEngine.GetRoot();

            var S_0_17 = CastAndCountChildren<ISymbolNode>(root, 2);
        }
        
        private static IGrammar CreateRegularExpressionStubGrammar()
        {
            ProductionBuilder R = "R", E = "E", T = "T", F = "F", A = "A", I = "I";
            R.Definition = (_)
                E 
                | '^' + E 
                | E + '$' 
                | '^' + E + '$';
            E.Definition = (_)
                T 
                | T + '|' + E
                | (_)null;
            T.Definition = (_)
                F + T 
                | F;
            F.Definition = (_)
                A
                | A + I;
            A.Definition = (_)
                'a';
            I.Definition = (_)
                '+' 
                | '?' 
                | '*';

            return new GrammarBuilder(R, new[] { R, E, T, F, A, I }).ToGrammar();
        }

        private static void AssertNodeProperties(ISymbolNode node, string nodeName, int origin, int location)
        {
            var actualNodeName = (node.Symbol as INonTerminal).Value;
            Assert.AreEqual(nodeName, actualNodeName, "Node Name Match Failed.");
            Assert.AreEqual(origin, node.Origin, "Origin Match Failed.");
            Assert.AreEqual(location, node.Location, "Location Match Failed.");
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
            var digit = new TerminalLexerRule(
                new DigitTerminal(),
                new TokenType("digit"));

            ProductionBuilder S = "S", M = "M", T = "T";
            S.Definition = S + '+' + M | M;
            M.Definition = M + '*' + T | T;
            T.Definition = digit;

            var grammar = new GrammarBuilder(S, new[] { S, M, T }).ToGrammar();
            return grammar;
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
