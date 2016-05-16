using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Forest;
using Pliant.Builders;
using Pliant.Charts;
using Pliant.Grammars;
using Pliant.Tokens;
using System.Collections.Generic;
using System.Linq;
using Pliant.Tests.Unit.Forest;

namespace Pliant.Tests.Unit
{
    [TestClass]
    public class ParseEngineTests
    {
        [TestMethod]
        public void ParseEngineGivenAmbiguousNullableRightRecursionShouldCreateMultipleParsePaths()
        {
            // example 1 section 3, Elizabeth Scott
            var tokens = Tokenize("aa");

            ProductionBuilder S = "S", T = "T", B = "B";
            S.Definition = S + T | "a";
            B.Definition = (_)null;
            T.Definition = "a" + B | "a";

            var grammar = new GrammarBuilder(S, new[] { S, T, B }).ToGrammar();
            var parseEngine = new ParseEngine(grammar, new ParseEngineOptions(optimizeRightRecursion: false));
            ParseInput(parseEngine, tokens);

            var actual = parseEngine.GetParseForestRoot() as IInternalForestNode;            
            
            var a_1_2 = new FakeTokenForestNode("a", 1, 2);
            var expected = 
                new FakeSymbolForestNode(S.LeftHandSide, 0, 2, 
                    new FakeAndForestNode(
                        new FakeSymbolForestNode(S.LeftHandSide, 0, 1, 
                            new FakeAndForestNode(
                                new FakeTokenForestNode("a", 0, 1))),
                        new FakeSymbolForestNode(T.LeftHandSide, 1, 2,
                            new FakeAndForestNode(
                                a_1_2),
                            new FakeAndForestNode(
                                a_1_2,
                                new FakeSymbolForestNode(B.LeftHandSide, 2, 2, new FakeAndForestNode(
                                    new FakeTokenForestNode("", 2,2)))))));
            AssertForestsAreEqual(expected, actual);
        }

        [TestMethod]
        public void ParseEngineGivenAmbiguousGrammarShouldCreateMulipleParsePaths()
        {
            // example 3 section 4, Elizabeth Scott
            var tokens = Tokenize("abbb");

            ProductionBuilder B = "B", S = "S", T = "T", A = "A";

            S.Definition = (_)A + T | 'a' + T;
            A.Definition = (_)'a' | B + A;
            B.Definition = null;
            T.Definition = (_)'b' + 'b' + 'b';

            var grammar = new GrammarBuilder(S, new[] { S, A, B, T })
                .ToGrammar();

            var parseEngine = new ParseEngine(grammar);
            ParseInput(parseEngine, tokens);
            
            var S_0_4 = parseEngine.GetParseForestRoot() as ISymbolForestNode;
            Assert.IsNotNull(S_0_4);
            AssertNodeProperties(S_0_4, nameof(S), 0, 4);
            Assert.AreEqual(2, S_0_4.Children.Count);

            var S_0_4_1 = S_0_4.Children[0] as IAndForestNode;
            Assert.IsNotNull(S_0_4_1);
            Assert.AreEqual(2, S_0_4_1.Children.Count);

            var a_0_1 = S_0_4_1.Children[0] as ITokenForestNode;
            Assert.IsNotNull(a_0_1);
            Assert.AreEqual("a", a_0_1.Token.Value);

            var T_1_4 = S_0_4_1.Children[1] as ISymbolForestNode;
            Assert.IsNotNull(T_1_4);
            AssertNodeProperties(T_1_4, nameof(T), 1, 4);
            Assert.AreEqual(1, T_1_4.Children.Count);

            var S_0_4_2 = S_0_4.Children[1] as IAndForestNode;
            Assert.IsNotNull(S_0_4_2);
            Assert.AreEqual(2, S_0_4_2.Children.Count);

            var A_0_1 = S_0_4_2.Children[0] as ISymbolForestNode;
            Assert.IsNotNull(A_0_1);
            AssertNodeProperties(A_0_1, nameof(A), 0, 1);
            Assert.AreEqual(2, A_0_1.Children.Count);

            var A_0_1_1 = A_0_1.Children[0] as IAndForestNode;
            Assert.IsNotNull(A_0_1_1);
            Assert.AreEqual(1, A_0_1_1.Children.Count);

            Assert.AreSame(a_0_1, A_0_1_1.Children[0]);

            var A_0_1_2 = A_0_1.Children[1] as IAndForestNode;
            Assert.IsNotNull(A_0_1_1);
            Assert.AreEqual(2, A_0_1_2.Children.Count);

            Assert.AreSame(A_0_1, A_0_1_2.Children[1]);

            var B_0_0 = A_0_1_2.Children[0] as ISymbolForestNode;
            Assert.IsNotNull(B_0_0);
            AssertNodeProperties(B_0_0, nameof(B), 0, 0);
            Assert.AreEqual(1, B_0_0.Children.Count);

            var B_0_0_1 = B_0_0.Children[0] as IAndForestNode;
            Assert.IsNotNull(B_0_0_1);
            Assert.AreEqual(1, B_0_0_1.Children.Count);

            var nullToken = B_0_0_1.Children[0] as ITokenForestNode;
            Assert.IsNotNull(nullToken);
            Assert.AreEqual(string.Empty, nullToken.Token.Value);

            var T_1_4_1 = T_1_4.Children[0] as IAndForestNode;
            Assert.IsNotNull(T_1_4_1);
            Assert.AreEqual(2, T_1_4_1.Children.Count);

            var T_1_3 = T_1_4_1.Children[0] as IIntermediateForestNode;
            Assert.IsNotNull(T_1_3);
            Assert.AreEqual(1, T_1_3.Children.Count);

            var b_3_4 = T_1_4_1.Children[1] as ITokenForestNode;
            Assert.IsNotNull(b_3_4);
            Assert.AreEqual("b", b_3_4.Token.Value);

            var T_1_3_1 = T_1_3.Children[0] as IAndForestNode;
            Assert.IsNotNull(T_1_3_1);
            Assert.AreEqual(2, T_1_3_1.Children.Count);

            var b_1_2 = T_1_3_1.Children[0] as ITokenForestNode;
            Assert.IsNotNull(b_1_2);
            Assert.AreEqual("b", b_1_2.Token.Value);

            var b_2_3 = T_1_3_1.Children[1] as ITokenForestNode;
            Assert.IsNotNull(b_2_3);
            Assert.AreEqual("b", b_2_3.Token.Value);
        }

        [TestMethod]
        public void ParseEngineWhenScanCompletedShouldCreateInternalAndTerminalNodes()
        {
            ProductionBuilder S = "S";
            S.Definition = (_)'a';

            var grammar = new GrammarBuilder(S, new[] { S })
                .ToGrammar();

            var tokens = Tokenize("a");
            var parseEngine = new ParseEngine(grammar);
            ParseInput(parseEngine, tokens);

            var parseNode = parseEngine.GetParseForestRoot();
            Assert.IsNotNull(parseNode);

            var S_0_1 = parseNode as ISymbolForestNode;
            Assert.IsNotNull(S_0_1);
            Assert.AreEqual(1, S_0_1.Children.Count);

            var S_0_1_1 = S_0_1.Children[0] as IAndForestNode;
            Assert.IsNotNull(S_0_1_1);
            Assert.AreEqual(1, S_0_1_1.Children.Count);

            var a_0_1 = S_0_1_1.Children[0] as ITokenForestNode;
            Assert.IsNotNull(a_0_1);
            Assert.AreEqual("a", a_0_1.Token.Value);
        }

        [TestMethod]
        public void ParseEnginePredicationShouldCreateInternalNode()
        {
            ProductionBuilder S = "S", A = "A";
            S.Definition = (_)A;
            A.Definition = (_)'a';

            var grammar = new GrammarBuilder(S, new[] { S, A }).ToGrammar();

            var tokens = Tokenize("a");
            var parseEngine = new ParseEngine(grammar);
            ParseInput(parseEngine, tokens);

            /*  S_0_1 -> A_0_1
             *  A_0_1 -> 'a'
             */
            var S_0_1 = parseEngine.GetParseForestRoot() as ISymbolForestNode;
            Assert.IsNotNull(S_0_1);
            Assert.AreEqual(1, S_0_1.Children.Count);

            var S_0_1_1 = S_0_1.Children[0] as IAndForestNode;
            Assert.IsNotNull(S_0_1_1);
            Assert.AreEqual(1, S_0_1_1.Children.Count);

            var A_0_1 = S_0_1_1.Children[0] as ISymbolForestNode;
            Assert.IsNotNull(A_0_1);
            Assert.AreEqual(1, A_0_1.Children.Count);

            var A_0_1_1 = A_0_1.Children[0] as IAndForestNode;
            Assert.IsNotNull(A_0_1_1);
            Assert.AreEqual(1, A_0_1_1.Children.Count);

            var a_0_1 = A_0_1_1.Children[0] as ITokenForestNode;
            Assert.IsNotNull(a_0_1);
            Assert.AreEqual("a", a_0_1.Token.Value);
        }

        [TestMethod]
        public void ParseEngineLeoItemsShouldGenerateProperParseTree()
        {
            ProductionBuilder S = "S", A = "A";

            S.Definition = A;
            A.Definition = 'a' + A | 'b';

            var grammar = new GrammarBuilder(S, new[] { S, A }).ToGrammar();

            var tokens = Tokenize("ab");
            var parseEngine = new ParseEngine(grammar);
            ParseInput(parseEngine, tokens);

            /*  S_0_2 -> A_0_2
             *  A_0_2 -> a_0_1 A_1_2
             *  A_1_2 -> b_1_2
             */
            var S_0_2 = parseEngine.GetParseForestRoot() as ISymbolForestNode;
            Assert.IsNotNull(S_0_2);
            Assert.AreEqual(1, S_0_2.Children.Count);

            var S_0_2_1 = S_0_2.Children[0] as IAndForestNode;
            Assert.IsNotNull(S_0_2_1);
            Assert.AreEqual(1, S_0_2_1.Children.Count);

            var A_0_2 = S_0_2_1.Children[0] as ISymbolForestNode;
            Assert.IsNotNull(A_0_2);
            Assert.AreEqual(1, A_0_2.Children.Count);

            var A_0_2_1 = A_0_2.Children[0] as IAndForestNode;
            Assert.IsNotNull(A_0_2_1);
            Assert.AreEqual(2, A_0_2_1.Children.Count);

            var a_0_1 = A_0_2_1.Children[0] as ITokenForestNode;
            Assert.IsNotNull(a_0_1);
            Assert.AreEqual("a", a_0_1.Token.Value);

            var A_1_2 = A_0_2_1.Children[1] as ISymbolForestNode;
            Assert.IsNotNull(A_1_2);
            Assert.AreEqual(1, A_1_2.Children.Count);

            var A_1_2_1 = A_1_2.Children[0] as IAndForestNode;
            Assert.IsNotNull(A_1_2_1);
            Assert.AreEqual(1, A_1_2_1.Children.Count);

            var b_1_2 = A_1_2_1.Children[0] as ITokenForestNode;
            Assert.IsNotNull(b_1_2);
            Assert.AreEqual("b", b_1_2.Token.Value);
        }

        [TestMethod]
        public void ParseEnginePassThroughRecursiveItemsShouldCreateVirtualNodes()
        {
            ProductionBuilder S = "S", A = "A", B = "B";
            S.Definition = A;
            A.Definition = 'a' + B;
            B.Definition = A | 'b';

            var grammar = new GrammarBuilder(S, new[] { S, A, B }).ToGrammar();
            var tokens = Tokenize("aaab");

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
            var S_0_4 = parseEngine.GetParseForestRoot() as ISymbolForestNode;
            Assert.IsNotNull(S_0_4);
            Assert.AreEqual(1, S_0_4.Children.Count);

            var S_0_4_1 = S_0_4.Children[0] as IAndForestNode;
            Assert.IsNotNull(S_0_4_1);
            Assert.AreEqual(1, S_0_4_1.Children.Count);

            var A_0_4 = S_0_4_1.Children[0] as ISymbolForestNode;
            Assert.IsNotNull(A_0_4);
            Assert.AreEqual(1, A_0_4.Children.Count);

            var A_0_4_1 = A_0_4.Children[0] as IAndForestNode;
            Assert.IsNotNull(A_0_4_1);
            Assert.AreEqual(2, A_0_4_1.Children.Count);

            var a_0_1 = A_0_4_1.Children[0] as ITokenForestNode;
            Assert.IsNotNull(a_0_1);
            Assert.AreEqual("a", a_0_1.Token.Value);

            var B_1_4 = A_0_4_1.Children[1] as ISymbolForestNode;
            Assert.IsNotNull(B_1_4);
            Assert.AreEqual(1, B_1_4.Children.Count);

            var B_1_4_1 = B_1_4.Children[0] as IAndForestNode;
            Assert.IsNotNull(B_1_4_1);
            Assert.AreEqual(1, B_1_4_1.Children.Count);

            var A_1_4 = B_1_4_1.Children[0] as ISymbolForestNode;
            Assert.IsNotNull(A_1_4);
            Assert.AreEqual(1, A_1_4.Children.Count);

            var A_1_4_1 = A_1_4.Children[0] as IAndForestNode;
            Assert.IsNotNull(A_1_4_1);
            Assert.AreEqual(2, A_1_4_1.Children.Count);

            var a_1_2 = A_1_4_1.Children[0] as ITokenForestNode;
            Assert.IsNotNull(a_1_2);
            Assert.AreEqual("a", a_1_2.Token.Value);

            var B_2_4 = A_1_4_1.Children[1] as ISymbolForestNode;
            Assert.IsNotNull(B_2_4);
            Assert.AreEqual(1, B_2_4.Children.Count);

            var B_2_4_1 = B_2_4.Children[0] as IAndForestNode;
            Assert.IsNotNull(B_2_4_1);
            Assert.AreEqual(1, B_2_4_1.Children.Count);

            var A_2_4 = B_2_4_1.Children[0] as ISymbolForestNode;
            Assert.IsNotNull(A_2_4);
            Assert.AreEqual(1, A_2_4.Children.Count);

            var A_2_4_1 = A_2_4.Children[0] as IAndForestNode;
            Assert.IsNotNull(A_2_4_1);
            Assert.AreEqual(2, A_2_4_1.Children.Count);

            var a_2_3 = A_2_4_1.Children[0] as ITokenForestNode;
            Assert.IsNotNull(a_2_3);
            Assert.AreEqual("a", a_2_3.Token.Value);

            var B_3_4 = A_2_4_1.Children[1] as ISymbolForestNode;
            Assert.IsNotNull(B_3_4);
            Assert.AreEqual(1, B_3_4.Children.Count);

            var B_3_4_1 = B_3_4.Children[0] as IAndForestNode;
            Assert.IsNotNull(B_3_4_1);
            Assert.AreEqual(1, B_3_4_1.Children.Count);

            var b_3_4 = B_3_4_1.Children[0] as ITokenForestNode;
            Assert.IsNotNull(b_3_4);
            Assert.AreEqual("b", b_3_4.Token.Value);
        }

        [TestMethod]
        public void ParseEngineShouldParseMidGrammarRightRecursionAndHandleNullRootTransitionItem()
        {
            ProductionBuilder S = "S", A = "A", B = "B", C = "C";
            S.Definition = (_)A | A + S;
            A.Definition = (_)B | B + C;
            B.Definition = (_)'.';
            C.Definition = (_)'+' | '?' | '*';

            var grammar = new GrammarBuilder(S, new[] { S, A, B, C }).ToGrammar();
            var tokens = Tokenize(".+");
            var parseEngine = new ParseEngine(grammar);
            ParseInput(parseEngine, tokens);

            var parseForest = parseEngine.GetParseForestRoot();
            Assert.IsNotNull(parseForest);

            // S_0_2 -> A_0_2
            // A_0_2 -> B_0_1 C_1_2
            // B_0_1 -> '.'_0_1
            // C_1_2 -> '+'_1_2
            var S_0_2 = parseForest as ISymbolForestNode;
            Assert.IsNotNull(S_0_2);
            Assert.AreEqual(1, S_0_2.Children.Count);

            var S_0_2_1 = S_0_2.Children[0] as IAndForestNode;
            Assert.IsNotNull(S_0_2_1);
            Assert.AreEqual(1, S_0_2_1.Children.Count);

            var A_0_2 = S_0_2_1.Children[0] as ISymbolForestNode;
            Assert.IsNotNull(A_0_2);
            Assert.AreEqual(1, A_0_2.Children.Count);

            var A_0_2_1 = A_0_2.Children[0] as IAndForestNode;
            Assert.IsNotNull(A_0_2_1);
            Assert.AreEqual(2, A_0_2_1.Children.Count);

            var B_0_1 = A_0_2_1.Children[0] as ISymbolForestNode;
            Assert.IsNotNull(B_0_1);
            Assert.AreEqual(1, B_0_1.Children.Count);

            var B_0_1_1 = B_0_1.Children[0] as IAndForestNode;
            Assert.IsNotNull(B_0_1_1);
            Assert.AreEqual(1, B_0_1_1.Children.Count);

            var dot_0_1 = B_0_1_1.Children[0] as ITokenForestNode;
            Assert.IsNotNull(dot_0_1);
            Assert.AreEqual(".", dot_0_1.Token.Value);

            var C_1_2 = A_0_2_1.Children[1] as ISymbolForestNode;
            Assert.IsNotNull(C_1_2);

            var C_1_2_1 = C_1_2.Children[0] as IAndForestNode;
            Assert.IsNotNull(C_1_2_1);
            Assert.AreEqual(1, C_1_2_1.Children.Count);

            var plus_1_2 = C_1_2_1.Children[0] as ITokenForestNode;
            Assert.IsNotNull(plus_1_2);
            Assert.AreEqual("+", plus_1_2.Token.Value);
        }

        [TestMethod]
        public void ParseEngineShouldParseSimpleSubstitutionGrammar()
        {
            ProductionBuilder A = "A", B = "B", C = "C";
            A.Definition = (_)B + C;
            B.Definition = (_)'b';
            C.Definition = (_)'c';

            var grammar = new GrammarBuilder(A, new[] { A, B, C }).ToGrammar();
            var parseEngine = new ParseEngine(grammar);

            var tokens = Tokenize("bc");
            ParseInput(parseEngine, tokens);
        }

        [TestMethod]
        public void ParseEngineShouldParseExpressionGrammar()
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
        public void ParseEngineWhenInvalidInputShouldExitParse()
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
        public void ParseEngineShouldParseUnmarkedMiddleRecursion()
        {
            ProductionBuilder S = "S";
            S.Definition = 'a' + S + 'a' | 'a';

            var grammar = new GrammarBuilder(S, new[] { S }).ToGrammar();
            var parseEngine = new ParseEngine(grammar);

            var tokens = Tokenize("aaaaaaaaa");
            ParseInput(parseEngine, tokens);
        }

        [TestMethod]
        public void ParseEngineShouldLeoOptimizeRightRecursiveQuasiCompleteItems()
        {
            ProductionBuilder S = "S", A = "A", B = "B";
            S.Definition = A + B;
            A.Definition = 'a' + A | 'a';
            B.Definition = 'b' + B | (_)null;

            var grammar = new GrammarBuilder(S, new[] { S, A, B }).ToGrammar();
            var input = Tokenize("aaaaaaaaaaaaaaaaaaabbbbbbbbbbb");
            var parseEngine = new ParseEngine(grammar);
            ParseInput(parseEngine, input);
            var chart = GetChartFromParseEngine(parseEngine);
            // when this count is < 10 we know that quasi complete items are being processed successfully
            Assert.IsTrue(chart.EarleySets[23].Completions.Count < 10);
        }

        [TestMethod]
        public void ParseEngineRightRecursionShouldNotBeCubicComplexity()
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
        public void ParseEngineGivenIntermediateStepsShouldCreateTransitionItems()
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
        public void ParseEngineShouldHandleTransitionsFromRightRecursionToNormalGrammar()
        {
            var grammar = CreateRegularExpressionStubGrammar();

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
            var R_0_4 = CastAndCountChildren<ISymbolForestNode>(parseEngine.GetParseForestRoot(), 1);
            AssertNodeProperties(R_0_4, "R", 0, 4);
            var E_0_4 = GetAndCastChildAtIndex<ISymbolForestNode>(R_0_4, 0);
            AssertNodeProperties(E_0_4, "E", 0, 4);
            var T_0_4 = GetAndCastChildAtIndex<ISymbolForestNode>(E_0_4, 0);
            AssertNodeProperties(T_0_4, "T", 0, 4);
            var F_0_1 = GetAndCastChildAtIndex<ISymbolForestNode>(T_0_4, 0);
            AssertNodeProperties(F_0_1, "F", 0, 1);
            var T_1_4 = GetAndCastChildAtIndex<ISymbolForestNode>(T_0_4, 1);
            AssertNodeProperties(T_1_4, "T", 1, 4);
            var F_1_2 = GetAndCastChildAtIndex<ISymbolForestNode>(T_1_4, 0);
            AssertNodeProperties(F_1_2, "F", 1, 2);
            var T_2_4 = GetAndCastChildAtIndex<ISymbolForestNode>(T_1_4, 1);
            AssertNodeProperties(T_2_4, "T", 2, 4);
            var F_2_4 = GetAndCastChildAtIndex<ISymbolForestNode>(T_2_4, 0);
            AssertNodeProperties(F_2_4, "F", 2, 3);
            var T_3_4 = GetAndCastChildAtIndex<ISymbolForestNode>(T_2_4, 1);
            AssertNodeProperties(T_3_4, "T", 3, 4);
            var F_3_4 = GetAndCastChildAtIndex<ISymbolForestNode>(T_3_4, 0);
            AssertNodeProperties(F_3_4, "F", 3, 4);
        }

        [TestMethod]
        public void ParseEngineWhenMultipleLeoItemsExistOnSearchPathShouldCreateCorrectParseTree()
        {
            var grammar = CreateRegularExpressionStubGrammar();
            var input = Tokenize("aaa");
            var parseEngine = new ParseEngine(grammar);
            ParseInput(parseEngine, input);
            
            var R_0_3 = CastAndCountChildren<ISymbolForestNode>(parseEngine.GetParseForestRoot(), 1);
            AssertNodeProperties(R_0_3, "R", 0, 3);
            var E_0_3 = GetAndCastChildAtIndex<ISymbolForestNode>(R_0_3, 0);
            AssertNodeProperties(E_0_3, "E", 0, 3);
            var T_0_3 = GetAndCastChildAtIndex<ISymbolForestNode>(E_0_3, 0);
            AssertNodeProperties(T_0_3, "T", 0, 3);
            var F_0_1 = GetAndCastChildAtIndex<ISymbolForestNode>(T_0_3, 0);
            AssertNodeProperties(F_0_1, "F", 0, 1);
            var A_0_1 = GetAndCastChildAtIndex<ISymbolForestNode>(F_0_1, 0);
            AssertNodeProperties(A_0_1, "A", 0, 1);
            var T_1_3 = GetAndCastChildAtIndex<ISymbolForestNode>(T_0_3, 1);
            AssertNodeProperties(T_1_3, "T", 1, 3);
            var F_1_2 = GetAndCastChildAtIndex<ISymbolForestNode>(T_1_3, 0);
            AssertNodeProperties(F_1_2, "F", 1, 2);
            var T_2_3 = GetAndCastChildAtIndex<ISymbolForestNode>(T_1_3, 1);
            AssertNodeProperties(T_2_3, "T", 2, 3);
            var F_2_3 = GetAndCastChildAtIndex<ISymbolForestNode>(T_2_3, 0);
            AssertNodeProperties(F_2_3, "F", 2, 3);
        }

        [TestMethod]
        public void ParseEngineGivenLongProductionRuleShouldCreateCorrectParseTree()
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
            var root = parseEngine.GetParseForestRoot();

            var S_0_17 = CastAndCountChildren<ISymbolForestNode>(root, 2);
        }

        [TestMethod]
        public void ParseEngineShouldCreateSameParseTreeForNullableRightRecursiveRule()
        {
            ProductionBuilder E = "E", F = "F";
            E.Definition =
                F
                | F + E
                | (_)null;
            F.Definition =
                new TerminalLexerRule(
                    new SetTerminal('a', 'b'), "[ab]");

            var grammar = new GrammarBuilder(E, new[] { E, F})
                .ToGrammar();

            var input = "ab";

            var leoEngine = new ParseEngine(grammar, new ParseEngineOptions(optimizeRightRecursion: true));
            var leoInterface = new ParseRunner(leoEngine, input);
            Assert.IsTrue(RunParse(leoInterface));

            var classicEngine = new ParseEngine(grammar, new ParseEngineOptions(optimizeRightRecursion: false));
            var classicInterface = new ParseRunner(classicEngine, input);
            Assert.IsTrue(RunParse(classicInterface));

            AssertForestsAreEqual(leoEngine.GetParseForestRoot(), classicEngine.GetParseForestRoot());
        }

        private static void AssertForestsAreEqual(IAndForestNode first, IAndForestNode second)
        {
            Assert.AreEqual(first.Children.Count, second.Children.Count);
            var firstChildrenEnumerator = first.Children.GetEnumerator();
            var secondChildrenEnumerator = second.Children.GetEnumerator();

            var firstChildEnumeratorMoveNext = false;
            var secondChildEnumeratorMoveNext = false;

            do
            {
                firstChildEnumeratorMoveNext = firstChildrenEnumerator.MoveNext();
                secondChildEnumeratorMoveNext = secondChildrenEnumerator.MoveNext();
                Assert.AreEqual(firstChildEnumeratorMoveNext, secondChildEnumeratorMoveNext);
                if(firstChildEnumeratorMoveNext)
                    AssertForestsAreEqual(firstChildrenEnumerator.Current, secondChildrenEnumerator.Current);
            } while (firstChildEnumeratorMoveNext);
        }

        private static void AssertForestsAreEqual(IInternalForestNode first, IInternalForestNode second)
        {
            Assert.AreEqual(first.Children.Count, second.Children.Count);
            var firstChildrenEnumerator = first.Children.GetEnumerator();
            var secondChildrenEnumerator = second.Children.GetEnumerator();

            var firstChildEnumeratorMoveNext = false;
            var secondChildEnumeratorMoveNext = false;

            do
            {
                firstChildEnumeratorMoveNext = firstChildrenEnumerator.MoveNext();
                secondChildEnumeratorMoveNext = secondChildrenEnumerator.MoveNext();
                Assert.AreEqual(firstChildEnumeratorMoveNext, secondChildEnumeratorMoveNext);
                if(firstChildEnumeratorMoveNext)
                    AssertForestsAreEqual(firstChildrenEnumerator.Current, secondChildrenEnumerator.Current);
            } while (firstChildEnumeratorMoveNext);
        }

        private static void AssertForestsAreEqual(IForestNode first, IForestNode second)
        {
            Assert.AreEqual(first.NodeType, second.NodeType);
            switch (first.NodeType)
            {
                case ForestNodeType.Intermediate:
                    var firstIntermediate = first as IIntermediateForestNode;
                    var secondIntermediate = second as IIntermediateForestNode;
                    Assert.AreEqual(firstIntermediate.State, secondIntermediate.State);
                    AssertForestsAreEqual(firstIntermediate, secondIntermediate);
                    break;

                case ForestNodeType.Symbol:
                    var firstSymbol = first as ISymbolForestNode;
                    var secondSymbol = second as ISymbolForestNode;
                    Assert.AreEqual(firstSymbol.Symbol, secondSymbol.Symbol);
                    AssertForestsAreEqual(firstSymbol, secondSymbol);
                    break;

                case ForestNodeType.Terminal:
                    var firstTerminal = first as ITerminalForestNode;
                    var secondTerminal = second as ITerminalForestNode;
                    Assert.AreEqual(firstTerminal.Capture, secondTerminal.Capture);
                    break;

                case ForestNodeType.Token:
                    var firstToken = first as ITokenForestNode;
                    var secondToken = second as ITokenForestNode;
                    Assert.AreEqual(firstToken.Token.TokenType.Id, secondToken.Token.TokenType.Id);
                    Assert.AreEqual(firstToken.Token.Value, secondToken.Token.Value);
                    break;
            }
        }
        
        private static bool RunParse(ParseRunner lexer)
        {
            while (!lexer.EndOfStream())
            {
                if (!lexer.Read())
                    return false;
            }
            return lexer.ParseEngine.IsAccepted();
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

        private static void AssertNodeProperties(ISymbolForestNode node, string nodeName, int origin, int location)
        {
            var actualNodeName = (node.Symbol as INonTerminal).Value;
            Assert.AreEqual(nodeName, actualNodeName, "Node Name Match Failed.");
            Assert.AreEqual(origin, node.Origin, "Origin Match Failed.");
            Assert.AreEqual(location, node.Location, "Location Match Failed.");
        }

        private static T CastAndCountChildren<T>(IForestNode node, int childCount)
            where T : class, IInternalForestNode
        {
            var tNode = node as T;
            Assert.IsNotNull(node);
            Assert.AreEqual(1, tNode.Children.Count);
            var firstAndNode = tNode.Children[0];
            Assert.IsNotNull(firstAndNode);
            Assert.AreEqual(childCount, firstAndNode.Children.Count);
            return tNode;
        }

        private static T GetAndCastChildAtIndex<T>(IInternalForestNode node, int index)
            where T : class, IForestNode
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

        private static IToken CreateDigitToken(int value, int position)
        {
            return new Token(value.ToString(), position, new TokenType("digit"));
        }

        private static IToken CreateCharacterToken(char character, int position)
        {
            return new Token(character.ToString(), position, new TokenType(character.ToString()));
        }

        private static IEnumerable<IToken> Tokenize(string input)
        {
            return input.Select((x, i) =>
                new Token(x.ToString(), i, new TokenType(x.ToString())));
        }

        private static IEnumerable<IToken> Tokenize(string input, string tokenType)
        {
            return input.Select((x, i) =>
                new Token(x.ToString(), i, new TokenType(tokenType)));
        }

        private static void ParseInput(IParseEngine parseEngine, IEnumerable<IToken> tokens)
        {
            foreach (var token in tokens)
                Assert.IsTrue(parseEngine.Pulse(token));
            Assert.IsTrue(parseEngine.IsAccepted());
        }
    }
}