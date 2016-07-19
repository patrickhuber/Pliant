using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Builders.Expressions;
using Pliant.Runtime;
using Pliant.Forest;

namespace Pliant.Tests.Unit.Forest
{
    [TestClass]
    public class MultiPassForestNodeVisitorStateManagerTests
    {
        [TestMethod]
        public void MultiPassForestNodeVisitorStateManagerShouldGenerateTwoTreesForAmbiguousGrammar()
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
            var parseEngine = new ParseEngine(grammar);
            var parseRunner = new ParseRunner(parseEngine, input);

            while (!parseRunner.EndOfStream())
            {
                Assert.IsTrue(parseRunner.Read());
            }
            Assert.IsTrue(parseEngine.IsAccepted());

            var parseForestRoot = parseEngine.GetParseForestRootNode();
            var multipassVisitor = new MultiPassForestNodeVisitorStateManager();
        }
    }
}
