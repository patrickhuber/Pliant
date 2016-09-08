using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Builders.Expressions;
using Pliant.Grammars;
using Pliant.RegularExpressions;
using Pliant.Runtime;
using Pliant.Tokens;

namespace Pliant.Tests.Unit.Runtime
{
    [TestClass]
    public class CompressedParseEngineTests
    {
        private static readonly IGrammar ExpressionGrammar = GetExpressionGrammar();
        private static readonly IGrammar NullableGrammar = GetNullableGrammar();

        [TestMethod]
        public void CompressedParseEngineCanParseRegex()
        {
            var regexGrammar = new RegexGrammar();
            var preComputedRegexGrammar = new PreComputedGrammar(regexGrammar);
            var parseEngine = new CompressedParseEngine(preComputedRegexGrammar);
            
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
        public void CompressedParseEngineCanParseNullableGrammar()
        {
            var preComputedGrammar = new PreComputedGrammar(NullableGrammar);
            var parseEngine = new CompressedParseEngine(preComputedGrammar);

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
        public void CompressedParseEngineShouldReturnExpectedLexerRulesGivenNullableGrammar()
        {
            AssertExpectedLexerRulesReturnedFromInitializedParseEngine(NullableGrammar, 1);
        }

        [TestMethod]
        public void CompressedParseEngineShouldReturnExpectedLexerRulesGivenExpressionGrammar()
        {
            AssertExpectedLexerRulesReturnedFromInitializedParseEngine(ExpressionGrammar, 4);
        }

        private static void AssertExpectedLexerRulesReturnedFromInitializedParseEngine(IGrammar grammar, int expectedCount)
        {
            var preComputedGrammar = new PreComputedGrammar(grammar);
            var parseEngine = new CompressedParseEngine(preComputedGrammar);

            var lexerRules = parseEngine.GetExpectedLexerRules();
            Assert.AreEqual(
                expectedCount,
                lexerRules.Count, $"Expected {expectedCount} lexerRule, Found {lexerRules.Count}");
        }

        private static IGrammar GetExpressionGrammar()
        {
            ProductionExpression
                S = nameof(S),
                E = nameof(E),
                T = nameof(T),
                F = nameof(F);

            S.Rule = E;
            E.Rule = E + '+' + T
                | E + '-' + T
                | T;
            T.Rule = T + '*' + F
                | T + '|' + F
                | F;
            F.Rule = '+' + F
                | '-' + F
                | 'n'
                | '(' + E + ')';

            var grammar = new GrammarExpression(S).ToGrammar();
            return grammar;
        }

        private static IGrammar GetNullableGrammar()
        {
            ProductionExpression
                SP = nameof(SP),
                S = nameof(S),
                A = nameof(A),
                E = nameof(E);

            SP.Rule = S;
            S.Rule = A + A + A + A;
            A.Rule = 'a' | E;
            E.Rule = null;

            var grammar = new GrammarExpression(SP).ToGrammar();
            return grammar;
        }
    }
}
