using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Builders.Expressions;
using Pliant.Grammars;
using Pliant.RegularExpressions;
using Pliant.Runtime;
using Pliant.Tests.Common;
using Pliant.Tokens;
using Pliant.Tests.Common.Grammars;
using Pliant.LexerRules;

namespace Pliant.Tests.Unit.Runtime
{
    [TestClass]
    public class DeterministicParseEngineTests
    {
        private static readonly IGrammar ExpressionGrammar = new ExpressionGrammar();
        private static readonly IGrammar NullableGrammar = new NullableGrammar();

        [TestMethod]
        public void DeterministicParseEngineCanParseRegex()
        {
            var regexGrammar = new RegexGrammar();
            var preComputedRegexGrammar = new PreComputedGrammar(regexGrammar);
            var parseEngine = new DeterministicParseEngine(preComputedRegexGrammar);
            
            var pattern = "[a-z][0-9]abc123";

            var openBracket = new TokenType("[");
            var notMeta = new TokenType("NotMeta"); // maybe make this token type a readonly property on the regex grammar?
            var notCloseBracket = new TokenType("NotCloseBracket"); // maybe make this token type a readonly property on the regex grammar?            
            var closeBracket = new TokenType("]");
            var dash = new TokenType("-");

            for (int i=0;i<pattern.Length;i++)
            {
                TokenType tokenType = null;
                switch (pattern[i])
                {
                    case '[':
                        tokenType = openBracket;
                        break;

                    case ']':
                        tokenType = closeBracket;
                        break;

                    case '-':
                        tokenType = dash;
                        break;

                    default:
                        if (i < 10)
                            tokenType = notCloseBracket;
                        else
                            tokenType = notMeta;
                        break;
                }
                var token = new Token(pattern[i].ToString(), i, tokenType);
                var result = parseEngine.Pulse(token);
                Assert.IsTrue(result, $"Error at position {i}");
            }
            Assert.IsTrue(parseEngine.IsAccepted(), "Parse was not accepted");
        }

        [TestMethod]
        public void DeterministicParseEngineCanParseNullableGrammar()
        {
            var preComputedGrammar = new PreComputedGrammar(NullableGrammar);
            var parseEngine = new DeterministicParseEngine(preComputedGrammar);

            var input = "aaaa";
            var tokenType = new TokenType("a");

            for (int i = 0; i < input.Length; i++)
            {
                var result = parseEngine.Pulse(new Token("a", i, tokenType));
                Assert.IsTrue(result, $"Error at position {i}");
            }

            Assert.IsTrue(parseEngine.IsAccepted(), "Parse was not accepted");
        }

        [TestMethod]
        public void DeterministicParseEngineShouldReturnExpectedLexerRulesGivenNullableGrammar()
        {
            AssertExpectedLexerRulesReturnedFromInitializedParseEngine(NullableGrammar, 1);
        }

        [TestMethod]
        public void DeterministicParseEngineShouldReturnExpectedLexerRulesGivenExpressionGrammar()
        {
            AssertExpectedLexerRulesReturnedFromInitializedParseEngine(ExpressionGrammar, 4);
        }

        [TestMethod]
        public void DeterministicParseEngineShouldParseInSubCubicTimeGivenRightRecursiveGrammar()
        {
            var a = new TerminalLexerRule(
                new CharacterTerminal('a'),
                new TokenType("a"));
            ProductionExpression A = "A";
            A.Rule =
                'a' + A
                | (Expr)null;

            var grammarExpression = new GrammarExpression(A, new[] { A });
            
            var parseTester = new ParseTester(
                new DeterministicParseEngine(
                    new PreComputedGrammar(grammarExpression.ToGrammar())));
            
            const string input = "aaaaa";
            parseTester.RunParse(input);
        }

        [TestMethod]
        public void DeterministicParseEngineShouldParseGrammarWithCycles()
        {
            ProductionExpression
                A = nameof(A),
                B = nameof(B),
                C = nameof(C);

            A.Rule = B | 'a';
            B.Rule = C | 'b';
            C.Rule = A | 'c';

            var grammar = new GrammarExpression(A, new[] { A, B, C })
                .ToGrammar();
            
            var parseTester = new ParseTester(
                new DeterministicParseEngine(
                    new PreComputedGrammar(grammar)));

            const string input = "a";
            parseTester.RunParse(input);
        }

        [TestMethod]
        public void DeterministicParseEngineShouldParseRepeatingRightRecursiveRule()
        {
            var number = new NumberLexerRule();
            var openBracket = new TerminalLexerRule('[');
            var closeBracket = new TerminalLexerRule(']');
            var comma = new TerminalLexerRule(',');

            ProductionExpression
                A = "A",
                V = "V",
                VR = "VR";

            A.Rule = openBracket + VR + closeBracket;
            VR.Rule = V
                | V + comma + VR
                | (Expr)null;
            V.Rule = number;

            var grammar = new GrammarExpression(
                A, new[] { A, V, VR }).ToGrammar();

            var determinisicParseEngine = new DeterministicParseEngine(grammar);

            var tokens = new[]
            {
                new Token("[", 0, openBracket.TokenType),
                new Token("1", 1, number.TokenType),
                new Token(",", 2, comma.TokenType),
                new Token("2", 3, number.TokenType),
                new Token("]", 4, closeBracket.TokenType)
            };

            for (var i = 0; i < tokens.Length; i++)
            {
                var result = determinisicParseEngine.Pulse(tokens[i]);
                if (!result)
                    Assert.Fail($"Failure parsing at position {determinisicParseEngine.Location}");
            }

            var accepted = determinisicParseEngine.IsAccepted();
            if (!accepted)
                Assert.Fail($"Input was not accepted.");
        }

        private static void AssertExpectedLexerRulesReturnedFromInitializedParseEngine(IGrammar grammar, int expectedCount)
        {
            var preComputedGrammar = new PreComputedGrammar(grammar);
            var parseEngine = new DeterministicParseEngine(preComputedGrammar);

            var lexerRules = parseEngine.GetExpectedLexerRules();
            Assert.AreEqual(
                expectedCount,
                lexerRules.Count, $"Expected {expectedCount} lexerRule, Found {lexerRules.Count}");
        }        
    }
}
