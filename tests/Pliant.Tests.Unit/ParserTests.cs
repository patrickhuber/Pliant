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
            var recognizer = new PulseRecognizer(grammar);

            RecognizeInput(recognizer, input);
        }

        [TestMethod]
        public void Test_Parser_That_Completed_Scan_Creates_Internal_And_Scan_Node()
        {
            const string input = "a";
            var grammar = new GrammarBuilder("S", p => p
                .Production("S", r => r
                    .Rule('a')))
                .GetGrammar();
            var recognizer = new PulseRecognizer(grammar);

            RecognizeInput(recognizer, input);
            
            var onlyCompletion = recognizer.Chart.EarleySets[1].Completions.FirstOrDefault();
            Assert.IsNotNull(onlyCompletion);
            
            var parseNode = onlyCompletion.ParseNode;
            Assert.IsNotNull(parseNode);

            var internalNode = parseNode as IInternalNode;
            Assert.IsNotNull(internalNode);
            Assert.AreNotEqual(0, internalNode.Children.Count);

            var terminalNode = internalNode.Children[0] as ITerminalNode;
            Assert.IsNotNull(terminalNode);

            Assert.AreEqual('a', terminalNode.Capture);
        }

        private void RecognizeInput(PulseRecognizer recognizer, string input)
        {
            foreach (var character in input)
                Assert.IsTrue(recognizer.Pulse(character));
            Assert.IsTrue(recognizer.IsAccepted());
        }
    }
}
