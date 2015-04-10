using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Pliant.Tests.Unit
{
    [TestClass]
    public class ParserTests
    {
        [TestMethod]
        public void Test_Parser_That_Ambiguous_Grammar_Creates_Multiple_Parse_Paths()
        {
            const string input = "bbb";
            var grammar = new GrammarBuilder("S", p=>p
                    .Production("S", r=>r
                        .Rule("S", "S")
                        .Rule('b')))
                .GetGrammar();
            var parser = new Parser(grammar);
            ParseInput(parser, input);

            var rootNode = parser.ParseTree();
        }

        [TestMethod]
        public void Test_Parser_That_Completed_Scan_Creates_Internal_And_Terminal_Node()
        {
            const string input = "a";
            var grammar = new GrammarBuilder("S", p => p
                    .Production("S", r => r
                        .Rule('a')))
                .GetGrammar();
            var parser = new Parser(grammar);

            ParseInput(parser, input);
            
            var parseNode = parser.ParseTree();
            Assert.IsNotNull(parseNode);

            var internalNode = parseNode as IInternalNode;
            Assert.IsNotNull(internalNode);
            Assert.AreNotEqual(0, internalNode.Children.Count);

            var terminalNode = internalNode.Children[0] as ITerminalNode;
            Assert.IsNotNull(terminalNode);

            Assert.AreEqual('a', terminalNode.Capture);
        }

        [TestMethod]
        public void Test_Parser_That_Completed_Prediction_Creates_Internal_Node()
        {
            const string input = "a";
            var grammar = new GrammarBuilder("S", p => p
                    .Production("S", r => r
                        .Rule("A"))
                    .Production("A", r => r
                        .Rule('a')))
                .GetGrammar();
            var parser = new Parser(grammar);
            ParseInput(parser, input);

            var rootNode = parser.ParseTree() as IInternalNode;
            Assert.IsNotNull(rootNode);
            Assert.AreEqual(1, rootNode.Children.Count);

            var childNode = rootNode.Children[0] as IInternalNode;
            Assert.IsNotNull(childNode);
            Assert.IsNotNull(childNode.Children.Count);

            var terminalNode = childNode.Children[0] as ITerminalNode;
            Assert.IsNotNull(terminalNode);
        }

        private void ParseInput(Parser parser, string input)
        {
            foreach (var character in input)
                Assert.IsTrue(parser.Pulse(character));
            Assert.IsTrue(parser.IsAccepted());
        }
    }
}
