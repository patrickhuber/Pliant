using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            var marpaParseEngine = new MarpaParseEngine(new RightRecursionGrammar());
            var input = "aaaaaaaaa";
            var tokens = input.Select((a, i) => 
            {
                var value = a.ToString();
                return new Token(value, i, new TokenType(value));
            }).ToArray();

            for (var t = 0; t < tokens.Length; t++)
            {
                var token = tokens[t];
                var result = marpaParseEngine.Pulse(token);
                if (!result)
                    Assert.Fail($"Error parsing at token position {t}");
            }
        }
    }
}
