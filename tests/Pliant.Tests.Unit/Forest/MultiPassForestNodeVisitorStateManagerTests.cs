using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Builders.Expressions;

namespace Pliant.Tests.Unit.Forest
{
    [TestClass]
    public class MultiPassForestNodeVisitorStateManagerTests
    {
        [TestMethod]
        public void MultiplePassTreeIteratorShouldGenerateTwoTreesForGrammarWithAmbiguousRoot()
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

            var grammarExpression = new GrammarExpression(S, new[] { S, A, B, C });
            var parseTester = new ParseTester(grammarExpression);
            parseTester.RunParse(input);

            var parseForestRoot = parseTester.ParseEngine.GetParseForestRootNode();
            Assert.AreEqual(1, parseForestRoot.Children.Count);
        }

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

        [TestMethod]
        public void MultiPassTreeIteratorShouldIterateParseTreesForNestedAmbiguities()
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

            var grammarExpression = new GrammarExpression(Z);
            var parseTester = new ParseTester(grammarExpression);
            parseTester.RunParse(input);
            
            var parseForestRoot = parseTester.ParseEngine.GetParseForestRootNode();
            Assert.AreEqual(1, parseForestRoot.Children.Count);
        }
    }
}
