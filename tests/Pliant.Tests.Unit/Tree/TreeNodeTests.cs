using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Tree;
using Pliant.Ast;
using Pliant.Builders;

namespace Pliant.Tests.Unit.Tree
{
    [TestClass]
    public class TreeNodeTests
    {
        [TestMethod]
        public void Test_TreeNode_That_Flattens_Intermediate_Nodes()
        {
            ProductionBuilder S="S", A = "A", B = "B", C = "C";
            S.Definition = A + B + C;
            A.Definition = 'a';
            B.Definition = 'b';
            C.Definition = 'c';
            var grammar = new GrammarBuilder(S, new[] { S, A, B, C }).ToGrammar();
            var parseEngine = new ParseEngine(grammar);
            var parseInterface = new ParseInterface(parseEngine, "abc");
            while (!parseInterface.EndOfStream())
            {
                Assert.IsTrue(parseInterface.Read());
            }
            Assert.IsTrue(parseEngine.IsAccepted());

            var parseForest = parseEngine.GetParseForestRoot();
            Assert.IsTrue(parseForest is IInternalNode);

            var stateManager = new NodeVisitorStateManager();
            var internalNode = parseForest as IInternalNode;
            var currentAndNode = stateManager.GetCurrentAndNode(internalNode);
            var treeNode = new InternalTreeNode(internalNode, currentAndNode, stateManager);
            int childCount = 0;
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
        public void Test_TreeNode_That_Ambiguous_Parse_Returns_First_Parse_Tree()
        {
            ProductionBuilder A = "A";
            A.Definition =
                A + '+' + A
                | A + '-' + A
                | 'a';
            var grammar = new GrammarBuilder(A, new[] { A }).ToGrammar();

            var input = "a+a+a";
            var parseEngine = new ParseEngine(grammar);
            var parseInterface = new ParseInterface(parseEngine, input); 
            while(!parseInterface.EndOfStream())
            {
                Assert.IsTrue(parseInterface.Read());
            }
            Assert.IsTrue(parseEngine.IsAccepted());

            var parseForest = parseEngine.GetParseForestRoot();

        }
    }
}
