using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Forest;
using Pliant.Grammars;
using Pliant.Tree;
using Pliant.Builders.Expressions;
using Pliant.Runtime;

namespace Pliant.Tests.Unit.Tree
{
    [TestClass]
    public class TreeNodeTests
    {
        [TestMethod]
        public void TreeNodeShouldFlattenIntermediateNodes()
        {
            ProductionExpression S = "S", A = "A", B = "B", C = "C";
            S.Rule = A + B + C;
            A.Rule = 'a';
            B.Rule = 'b';
            C.Rule = 'c';
            var grammar = new GrammarExpression(S, new[] { S, A, B, C }).ToGrammar();
            var input = "abc";
            var treeNode = GetTreeNode(grammar, input);
            var childCount = 0;
            foreach (var child in treeNode.Children)
            {
                childCount++;
                Assert.AreEqual(TreeNodeType.Internal, child.NodeType);
                var internalChild = child as IInternalTreeNode;
                var grandChildCount = 0;
                foreach (var grandChild in internalChild.Children)
                {
                    grandChildCount++;
                    Assert.AreEqual(TreeNodeType.Token, grandChild.NodeType);
                }
                Assert.AreEqual(1, grandChildCount);
            }
            Assert.AreEqual(3, childCount);
        }

        [TestMethod]
        public void TreeNodeWhenAmbiguousParseShouldReturnFirstParseTree()
        {
            ProductionExpression A = "A";
            A.Rule =
                A + '+' + A
                | A + '-' + A
                | 'a';
            var grammar = new GrammarExpression(A, new[] { A }).ToGrammar();

            var input = "a+a+a";
            var treeNode = GetTreeNode(grammar, input);
        }

        private static InternalTreeNode GetTreeNode(IGrammar grammar, string input)
        {
            var parseEngine = new ParseEngine(grammar);
            var parseRunner = new ParseRunner(parseEngine, input);
            while (!parseRunner.EndOfStream())
            {
                Assert.IsTrue(parseRunner.Read());
            }
            Assert.IsTrue(parseEngine.IsAccepted());
            
            var parseForest = parseEngine.GetParseForestRootNode();

            Assert.IsTrue(parseForest is IInternalForestNode);

            var internalNode = parseForest as IInternalForestNode;

            var disambiguationAlgorithm = new SelectFirstChildDisambiguationAlgorithm();
            var treeNode = new InternalTreeNode(internalNode, disambiguationAlgorithm);
            return treeNode;            
        }
    }
}