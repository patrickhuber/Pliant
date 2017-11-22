using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Forest;
using Pliant.Charts;
using Pliant.Grammars;
using Pliant.Tokens;
using System.Collections.Generic;
using System.Linq;
using Pliant.Builders.Expressions;
using Pliant.Runtime;
using Pliant.Tree;
using Pliant.Tests.Common.Forest;
using Pliant.Tests.Common;
using Pliant.Tests.Common.Grammars;
using Testable;

namespace Pliant.Tests.Unit.Runtime
{
    [TestClass]
    public class ParseEngineTests
    {
        [TestMethod]
        public void ParseEngineGivenAmbiguousNullableRightRecursionShouldCreateMultipleParsePaths()
        {
            // example 1 section 3, Elizabeth Scott
            var tokens = Tokenize("aa");

            ProductionExpression S = "S", T = "T", B = "B";
            S.Rule = S + T | "a";
            B.Rule = null;
            T.Rule = "a" + B | "a";

            var grammar = new GrammarExpression(S, new[] { S, T, B }).ToGrammar();
            var parseEngine = new ParseEngine(grammar, new ParseEngineOptions(optimizeRightRecursion: false));
            ParseInput(parseEngine, tokens);

            var parseForestRoot = parseEngine.GetParseForestRootNode();
            var actual = parseForestRoot as IInternalForestNode;            
            
            var a_1_2 = new FakeTokenForestNode("a", 1, 2);
            var expected = 
                new FakeSymbolForestNode(S.ProductionModel.LeftHandSide.NonTerminal, 0, 2, 
                    new FakeAndForestNode(
                        new FakeSymbolForestNode(S.ProductionModel.LeftHandSide.NonTerminal, 0, 1, 
                            new FakeAndForestNode(
                                new FakeTokenForestNode("a", 0, 1))),
                        new FakeSymbolForestNode(T.ProductionModel.LeftHandSide.NonTerminal, 1, 2,
                            new FakeAndForestNode(
                                a_1_2),
                            new FakeAndForestNode(
                                a_1_2,
                                new FakeSymbolForestNode(B.ProductionModel.LeftHandSide.NonTerminal, 2, 2, new FakeAndForestNode(
                                    new FakeTokenForestNode("", 2,2)))))));
            AssertForestsAreEqual(expected, actual);
        }

        [TestMethod]
        public void ParseEngineGivenAmbiguousGrammarShouldCreateMulipleParsePaths()
        {
            // example 3 section 4, Elizabeth Scott
            var tokens = Tokenize("abbb");

            ProductionExpression B = "B", S = "S", T = "T", A = "A";

            S.Rule = (Expr)A + T | 'a' + T;
            A.Rule = (Expr)'a' | B + A;
            B.Rule = null;
            T.Rule = (Expr)'b' + 'b' + 'b';

            var grammar = new GrammarExpression(S, new[] { S, A, B, T })
                .ToGrammar();

            var parseEngine = new ParseEngine(grammar);
            ParseInput(parseEngine, tokens);

            var S_0_4 = parseEngine.GetParseForestRootNode() as ISymbolForestNode;
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
            ProductionExpression S = "S";
            S.Rule = (Expr)'a';

            var grammar = new GrammarExpression(S, new[] { S })
                .ToGrammar();

            var tokens = Tokenize("a");
            var parseEngine = new ParseEngine(grammar);
            ParseInput(parseEngine, tokens);

            var parseNode = parseEngine.GetParseForestRootNode();
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
            ProductionExpression S = "S", A = "A";
            S.Rule = (Expr)A;
            A.Rule = (Expr)'a';

            var grammar = new GrammarExpression(S, new[] { S, A }).ToGrammar();

            var tokens = Tokenize("a");
            var parseEngine = new ParseEngine(grammar);
            ParseInput(parseEngine, tokens);

            /*  S_0_1 -> A_0_1
             *  A_0_1 -> 'a'
             */
            var S_0_1 = parseEngine.GetParseForestRootNode() as ISymbolForestNode;
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
            ProductionExpression S = "S", A = "A";

            S.Rule = A;
            A.Rule = 'a' + A | 'b';

            var grammar = new GrammarExpression(S, new[] { S, A }).ToGrammar();

            var tokens = Tokenize("ab");
            var parseEngine = new ParseEngine(grammar);
            ParseInput(parseEngine, tokens);

            /*  S_0_2 -> A_0_2
             *  A_0_2 -> a_0_1 A_1_2
             *  A_1_2 -> b_1_2
             */
            var parseForestRoot = parseEngine.GetParseForestRootNode();
            var root = parseForestRoot;

            var S_0_2 = root as ISymbolForestNode;
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
            ProductionExpression S = "S", A = "A", B = "B";
            S.Rule = A;
            A.Rule = 'a' + B;
            B.Rule = A | 'b';

            var grammar = new GrammarExpression(S, new[] { S, A, B }).ToGrammar();
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
            var parseForestRoot = parseEngine.GetParseForestRootNode();
            var root = parseForestRoot;

            var S_0_4 = root as ISymbolForestNode;
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
            ProductionExpression S = "S", A = "A", B = "B", C = "C";
            S.Rule = (Expr)A | A + S;
            A.Rule = (Expr)B | B + C;
            B.Rule = (Expr)'.';
            C.Rule = (Expr)'+' | '?' | '*';

            var grammar = new GrammarExpression(S, new[] { S, A, B, C }).ToGrammar();
            var tokens = Tokenize(".+");
            var parseEngine = new ParseEngine(grammar);
            ParseInput(parseEngine, tokens);

            var parseForestRoot = parseEngine.GetParseForestRootNode();
            var parseForest = parseForestRoot;
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
            ProductionExpression A = "A", B = "B", C = "C";
            A.Rule = (Expr)B + C;
            B.Rule = (Expr)'b';
            C.Rule = (Expr)'c';

            var grammar = new GrammarExpression(A, new[] { A, B, C }).ToGrammar();
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
            ProductionExpression S = "S";
            S.Rule = 'a' + S + 'a' | 'a';

            var grammar = new GrammarExpression(S, new[] { S }).ToGrammar();
            var parseEngine = new ParseEngine(grammar);

            var tokens = Tokenize("aaaaaaaaa");
            ParseInput(parseEngine, tokens);
        }

        [TestMethod]
        public void ParseEngineShouldLeoOptimizeRightRecursiveQuasiCompleteItems()
        {
            ProductionExpression S = "S", A = "A", B = "B";
            S.Rule = A + B;
            A.Rule = 'a' + A | 'a';
            B.Rule = 'b' + B | (Expr)null;

            var grammar = new GrammarExpression(S, new[] { S, A, B }).ToGrammar();
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
            ProductionExpression A = "A";
            A.Rule =
                'a' + A
                | (Expr)null;

            var grammar = new GrammarExpression(A, new[] { A })
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
        public void ParseEngineShouldHandleCyclesInGrammar()
        {
            const string input = "a";
            var tokens = Tokenize(input);
            var recognizer = new ParseEngine(new CycleGrammar());
            ParseInput(recognizer, tokens);
        }

        [TestMethod]
        public void ParseEngineShouldHandleHiddenRightRecursionsInSubCubicTime()
        {
            const string input = "abcabcabcabcabcabca";
            var tokens = Tokenize(input);
            var recognizer = new ParseEngine(new HiddenRightRecursionGrammar());
            ParseInput(recognizer, tokens);
        }

        [TestMethod]
        public void ParseEngineGivenIntermediateStepsShouldCreateTransitionItems()
        {
            ProductionExpression S = "S", A = "A", B = "B";
            S.Rule = A;
            A.Rule = 'a' + B;
            B.Rule = A | 'b';
            var grammar = new GrammarExpression(S, new[] { S, A, B }).ToGrammar();
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
            
            var parseForestRoot = parseEngine.GetParseForestRootNode();
            var root = parseForestRoot;
            var R_0_4 = CastAndCountChildren<ISymbolForestNode>(root, 1);
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

            var parseForestRoot = parseEngine.GetParseForestRootNode();
            var parseForest = parseForestRoot;

            var R_0_3 = CastAndCountChildren<ISymbolForestNode>(parseForest, 1);
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
            ProductionExpression S = "S", A = "A", B = "B", C = "C", D = "D";
            S.Rule = (Expr)
                A + B + C + D + S
                | '|';
            A.Rule = 'a';
            B.Rule = 'b';
            C.Rule = 'c';
            D.Rule = 'd';
            var grammar = new GrammarExpression(S, new[] { S, A, B, C, D }).ToGrammar();

            var input = Tokenize("abcdabcdabcdabcd|");
            var parseEngine = new ParseEngine(grammar);
            ParseInput(parseEngine, input);

            var parseForestRoot = parseEngine.GetParseForestRootNode();
            var pasreForestNode = parseForestRoot;

            var S_0_17 = CastAndCountChildren<ISymbolForestNode>(pasreForestNode, 2);
        }

        [TestMethod]
        public void ParseEngineShouldCreateSameParseTreeForNullableRightRecursiveRule()
        {
            ProductionExpression E = "E", F = "F";
            E.Rule =
                F
                | F + E
                | (Expr)null;
            F.Rule =
                new TerminalLexerRule(
                    new SetTerminal('a', 'b'), "[ab]");

            var grammar = new GrammarExpression(E, new[] { E, F})
                .ToGrammar();

            var input = "aba";
            AssertLeoAndClassicParseAlgorithmsCreateSameForest(input, grammar);
        }

        [TestMethod]
        public void ParseEngineAmbiguousRootShouldCreateSameLeoAndClassicForest()
        {
            ProductionExpression
                S = "S",
                A = "A",
                B = "B",
                C = "C";

            S.Rule = A | B;
            A.Rule = 'a' + C;
            B.Rule = 'a' + C;
            C.Rule = 'c';

            const string input = "ac";

            var grammar = new GrammarExpression(S, new[] { S, A, B, C }).ToGrammar();
            AssertLeoAndClassicParseAlgorithmsCreateSameForest(input, grammar);
        }

        [TestMethod]
        public void ParseEngineAmbiguousNestedChildrenShouldCreateSameLeoAndClassicForest()
        {
            ProductionExpression
                Z = "Z",
                S = "S",
                A = "A",
                B = "B",
                C = "C",
                D = "D",
                E = "E",
                F = "F";

            Z.Rule = S;
            S.Rule = A | B;
            A.Rule = '0' + C;
            B.Rule = '0' + C;
            C.Rule = D | E;
            D.Rule = '1' + F;
            E.Rule = '1' + F;
            F.Rule = '2';

            const string input = "012";
            
            var grammar = new GrammarExpression(S, new[] { S, A, B, C, D, E, F }).ToGrammar();
            AssertLeoAndClassicParseAlgorithmsCreateSameForest(input, grammar);
        }

        [TestMethod]
        public void ParseEngineDivergentAmbiguousGrammarShouldCreateSameLeoAndClassicParseForest()
        {
            ProductionExpression 
                S = "S", 
                A = "A", 
                B = "B",
                C = "C", 
                X = "X",
                Y = "Y", 
                Z = "Z";
            S.Rule = 
                '0' + A 
                | '0' + X;
            A.Rule = '1' + B;
            B.Rule = '2' + C;
            C.Rule = '3';
            X.Rule = '1' + Y;
            Y.Rule = '2' + Z;
            Z.Rule = '3';

            const string input = "0123";

            var grammar = new GrammarExpression(
                S, 
                new[] { S, A, B, C, X, Y, Z })
                .ToGrammar();
            AssertLeoAndClassicParseAlgorithmsCreateSameForest(input, grammar);
        }

        [TestMethod]
        public void ParseEngineShouldProduceSameLeoAndClassicParseForestWhenGivenLongAmbiguousProduction()
        {
            ProductionExpression
                S = "S", 
                A = "A", B = "B", C = "C", D= "D", 
                W = "W", X = "X", Y = "Y", Z = "Z";
            S.Rule = 
                A + B + C + D
                | W + X + Y + Z;
            A.Rule = '0';
            B.Rule = '1';
            C.Rule = '2';
            D.Rule = '3';
            W.Rule = '0';
            X.Rule = '1';
            Y.Rule = '2';
            Z.Rule = '3';

            var grammar = new GrammarExpression(
                S, 
                new[] { S, A, B, C, D, W, X, Y, Z })
                .ToGrammar();
            AssertLeoAndClassicParseAlgorithmsCreateSameForest("0123", grammar);
        }

        [TestMethod]
        public void ParseEngineShouldProduceSameLeoAndClassicForestWhenGivenAmbiguousNonTerminal()
        {
            var input = "1+2+3";
            var grammar = new SimpleExpressionGrammar();
            AssertLeoAndClassicParseAlgorithmsCreateSameForest(input, grammar);
        }

        [TestMethod]
        public void ParseEngineShouldDisambiguateFollowingOperatorPresidence()
        {
            var input = "2*3+5*7";
            var parseTester = new ParseTester(new ExpressionGrammar());
            parseTester.RunParse(input);
            var forest = parseTester.ParseEngine.GetParseForestRootNode();
            var tree = new InternalTreeNode(forest);
            // ((2*3)+(5*7))
            // (E, 0, 7) = (E, 0, 5) ('*', 5, 6) (E, 6, 7)
            // (E, 0, 5) = (E, 0, 3) ('+', 3, 4) (E, 4, 5)
            // (E, 0, 3) = (E, 0, 1) ('*', 1, 2) (E, 2, 3)
            // (E, 0, 1) = ('2', 0, 1)
            // (E, 2, 3) = ('3', 2, 3)
            // (E, 4, 5) = ('5', 4, 5)
            // (E, 6, 7) = ('7', 6, 7)
            Assert.Inconclusive();
        }

        [TestMethod]
        public void ParseEngineCanParseNullableGrammar()
        {
            new ParseTester(
                new NullableGrammar())
                .RunParse("aaaa");
        }
                        
        private static void AssertLeoAndClassicParseAlgorithmsCreateSameForest(string input, IGrammar grammar)
        {
            var leoEngine = new ParseEngine(grammar);
            var classicEngine = new ParseEngine(grammar, new ParseEngineOptions(optimizeRightRecursion: false));

            var leoTester = new ParseTester(leoEngine);
            var classicTester = new ParseTester(classicEngine);

            leoTester.RunParse(input);
            classicTester.RunParse(input);

            var nodeComparer = new StatefulForestNodeComparer();
            var leoParseForestRoot = leoEngine.GetParseForestRootNode();
            var classicParseForestRoot = classicEngine.GetParseForestRootNode();
            Assert.IsTrue(nodeComparer.Equals(
                classicParseForestRoot,
                leoParseForestRoot),
                "Leo and Classic Parse Forest mismatch");
        }

        static void AssertForestsAreEqual(IForestNode expected, IForestNode actual)
        {
            var comparer = new StatefulForestNodeComparer();
            Assert.IsTrue(comparer.Equals(expected, actual));
        }
                
        private static IGrammar CreateRegularExpressionStubGrammar()
        {
            ProductionExpression R = "R", E = "E", T = "T", F = "F", A = "A", I = "I";
            R.Rule = (Expr)
                E
                | '^' + E
                | E + '$'
                | '^' + E + '$';
            E.Rule = (Expr)
                T
                | T + '|' + E
                | (Expr)null;
            T.Rule = (Expr)
                F + T
                | F;
            F.Rule = (Expr)
                A
                | A + I;
            A.Rule = (Expr)
                'a';
            I.Rule = (Expr)
                '+'
                | '?'
                | '*';

            return new GrammarExpression(R, new[] { R, E, T, F, A, I }).ToGrammar();
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

            ProductionExpression S = "S", M = "M", T = "T";
            S.Rule = S + '+' + M | M;
            M.Rule = M + '*' + T | T;
            T.Rule = digit;

            var grammar = new GrammarExpression(S, new[] { S, M, T }).ToGrammar();
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

        private static IReadOnlyList<IToken> Tokenize(string input)
        {
            return input.Select((x, i) =>
                new Token(x.ToString(), i, new TokenType(x.ToString())))
                .ToArray();
        }
        
        private static void ParseInput(IParseEngine parseEngine, IReadOnlyList<IToken> tokens)
        {
            var parseTester = new ParseTester(parseEngine);
            parseTester.RunParse(tokens);
        }
    }
}