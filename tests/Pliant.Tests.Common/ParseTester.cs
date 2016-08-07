using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Builders.Expressions;
using Pliant.Grammars;
using Pliant.Runtime;
using Pliant.Tokens;
using System.Collections.Generic;

namespace Pliant.Tests.Common
{
    public class ParseTester
    {
        public IGrammar Grammar { get; private set; }
        public IParseEngine ParseEngine { get; private set; }
        public IParseRunner ParseRunner { get; private set; }

        public ParseTester(GrammarExpression expression)
        {
            Grammar = expression.ToGrammar();
            ParseEngine = new ParseEngine(Grammar);
        }

        public void RunParse(string input)
        {
            ParseRunner = new ParseRunner(ParseEngine, input);
            while (!ParseRunner.EndOfStream())
            {
                Assert.IsTrue(ParseRunner.Read(), $"Parse Failed at Position {ParseRunner.Position}");
            }
            Assert.IsTrue(ParseRunner.ParseEngine.IsAccepted(), $"Parse was not accepted");
        }
        
        public void Reset()
        {
            ParseEngine = new ParseEngine(Grammar);            
        }
    }
}
