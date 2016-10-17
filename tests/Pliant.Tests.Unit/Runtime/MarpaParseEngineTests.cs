using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Charts;
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

            var stateFrameChart = parseEngine.Chart;
            var lastFrameSet = stateFrameChart.FrameSets[stateFrameChart.FrameSets.Count - 1];
            Assert.AreEqual(4, lastFrameSet.Frames.Count);
        }
    }
}
