using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Builders.Expressions;

namespace Pliant.Tests.Unit.Forest
{
    [TestClass]
    public class MultiPassForestNodeVisitorStateManagerTests
    {
        [TestMethod]
        public void MultiPassTreeIteratorShouldIterateParseTreesForLeafAmbiguities()
        {
            ProductionExpression
                E = "E",
                F = "F";
            E.Rule =
                F
                | F + E
                | (Expr)null;
            F.Rule = 'a';

            const string input = "aaa";
            
            var grammarExpression = new GrammarExpression(E, new[] { E, F });
            var parseTester = new ParseTester(grammarExpression);
            parseTester.RunParse(input);
            
            var parseForestRoot = parseTester.ParseEngine.GetParseForestRootNode();
            Assert.AreEqual(1, parseForestRoot.Children.Count);
        }        
    }
}
