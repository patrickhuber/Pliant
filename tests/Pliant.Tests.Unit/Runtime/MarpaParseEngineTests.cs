using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Builders.Expressions;
using Pliant.Charts;
using Pliant.Grammars;
using Pliant.LexerRules;
using Pliant.RegularExpressions;
using Pliant.Runtime;
using Pliant.Tests.Common.Grammars;
using Pliant.Tokens;
using System.Linq;

namespace Pliant.Tests.Unit.Runtime
{
    [TestClass]
    public class MarpaParseEngineTests
    {
        [TestMethod]
        public void MarpaParseEngineShouldParseRightRecursiveGrammarInLinearTime()
        {
            var parseEngine = new MarpaParseEngine(new RightRecursionGrammar());
            var input = "aaaaaaaaa";
            var tokens = input.Select((a, i) => 
            {
                var value = a.ToString();
                return new Token(value, i, new TokenType(value));
            }).ToArray();

            for (var t = 0; t < tokens.Length; t++)
            {
                var token = tokens[t];
                var result = parseEngine.Pulse(token);
                if (!result)
                    Assert.Fail($"Error parsing at token position {t}");
            }
            var accepted = parseEngine.IsAccepted();
            if (!accepted)
                Assert.Fail($"Parse is not accepted. ");

            var deterministicChart = parseEngine.Chart;
            var lastDeterministicSet = deterministicChart.Sets[deterministicChart.Sets.Count - 1];
            Assert.AreEqual(4, lastDeterministicSet.States.Count);
        }

        [TestMethod]
        public void MarpaParseEngineCanParseRegex()
        {
            var regexGrammar = new RegexGrammar();
            var preComputedRegexGrammar = new PreComputedGrammar(regexGrammar);
            var parseEngine = new MarpaParseEngine(preComputedRegexGrammar);

            var pattern = "[a-z][0-9]abc123";

            var openBracket = new TokenType("[");
            var notMeta = new TokenType("NotMeta"); // maybe make this token type a readonly property on the regex grammar?
            var notCloseBracket = new TokenType("NotCloseBracket"); // maybe make this token type a readonly property on the regex grammar?            
            var closeBracket = new TokenType("]");
            var dash = new TokenType("-");

            for (int i = 0; i < pattern.Length; i++)
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
        public void MarpaParseEngineShouldParseEncapsulatedRepeatingRightRecursiveRule()
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
                A, new []{ A, V, VR }).ToGrammar();

            var marpaParseEngine = new MarpaParseEngine(grammar);

            var tokens = new [] 
            {
                new Token("[", 0, openBracket.TokenType),
                new Token("1", 1, number.TokenType),
                new Token(",", 2, comma.TokenType),
                new Token("2", 3, number.TokenType),
                new Token("]", 4, closeBracket.TokenType)
            };

            for (var i = 0; i < tokens.Length; i++)
            {
                var result = marpaParseEngine.Pulse(tokens[i]);
                if (!result)
                    Assert.Fail($"Failure parsing at position {marpaParseEngine.Location}");
            }

            var accepted = marpaParseEngine.IsAccepted();
            if (!accepted)
                Assert.Fail($"Input was not accepted.");
        }

        [TestMethod]
        public void MarpaParseEngineShouldNotMemoizeRuleWhenSiblingIsRightRecursiveAndCurrentRuleIsNot()
        {
            ProductionExpression
                S = "S",
                A = "A";

            S.Rule = A;
            A.Rule = (Expr)'a' + 'a' + A
                | (Expr) 'b' + 'b';

            var grammar = new GrammarExpression(S, new[] { S, A }).ToGrammar();
            var marpaParseEngine = new MarpaParseEngine(grammar);

            var bTokenType = new TokenType("b");
            var tokens = new[] 
            {
                new Token("b", 0, bTokenType),
                new Token("b", 1, bTokenType)
            };
            
            for (var i = 0; i < tokens.Length; i++)
            {
                var result = marpaParseEngine.Pulse(tokens[i]);
                if (!result)
                    Assert.Fail($"Failure parsing at position {marpaParseEngine.Location}");
            }

            var accepted = marpaParseEngine.IsAccepted();
            if (!accepted)
                Assert.Fail($"Input was not accepted.");
        }     
    }
}
