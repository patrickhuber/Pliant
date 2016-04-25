using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Forest;
using Pliant.Builders;
using Pliant.Grammars;
using Pliant.Tree;

namespace Pliant.Tests.Unit.Tree
{
    [TestClass]
    public class TreeNodeTests
    {
        [TestMethod]
        public void TreeNodeShouldFlattenIntermediateNodes()
        {
            ProductionBuilder S = "S", A = "A", B = "B", C = "C";
            S.Definition = A + B + C;
            A.Definition = 'a';
            B.Definition = 'b';
            C.Definition = 'c';
            var grammar = new GrammarBuilder(S, new[] { S, A, B, C }).ToGrammar();
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
            ProductionBuilder A = "A";
            A.Definition =
                A + '+' + A
                | A + '-' + A
                | 'a';
            var grammar = new GrammarBuilder(A, new[] { A }).ToGrammar();

            var input = "a+a+a";
            var treeNode = GetTreeNode(grammar, input);
        }

        private static InternalTreeNode GetTreeNode(IGrammar grammar, string input)
        {
            var parseEngine = new ParseEngine(grammar);
            var lexer = new Lexer(parseEngine, input);
            while (!lexer.EndOfStream())
            {
                Assert.IsTrue(lexer.Read());
            }
            Assert.IsTrue(parseEngine.IsAccepted());

            var parseForest = parseEngine.GetParseForestRoot();
            Assert.IsTrue(parseForest is IInternalNode);

            var internalNode = parseForest as IInternalNode;

            var stateManager = new MultiPassNodeVisitorStateManager();
            var currentAndNode = stateManager.GetCurrentAndNode(internalNode);
            var treeNode = new InternalTreeNode(internalNode, currentAndNode, stateManager);
            return treeNode;
        }
    }
}