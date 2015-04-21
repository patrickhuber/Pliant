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
            // example 3 section 4, Elizabeth Scott
            const string input = "abbb";
            var grammar = new GrammarBuilder("S", p => p
                    .Production("S", r => r
                        .Rule("A", "T")
                        .Rule('a', "T"))
                    .Production("A", r=>r
                        .Rule('a')
                        .Rule("B", "A"))
                    .Production("B", r => r
                        .Lambda())
                    .Production("T", r=>r
                        .Rule('b', 'b', 'b')))
                .GetGrammar();
            var T_Production = grammar.Productions[3];

            var parser = new Parser(grammar);
            ParseInput(parser, input);
                 
            var S_0_4 = parser.ParseTree() as ISymbolNode;
            Assert.IsNotNull(S_0_4);
            Assert.AreEqual(2, S_0_4.Children.Count);

            var S_0_4_1 = S_0_4.Children[0] as IAndNode;
            Assert.IsNotNull(S_0_4_1);
            Assert.AreEqual(2, S_0_4_1.Children.Count);

            var a_0_1 = S_0_4_1.Children[0] as ITerminalNode;
            Assert.IsNotNull(a_0_1);
            Assert.AreEqual('a', a_0_1.Capture);

            var T_1_4 = S_0_4_1.Children[1] as ISymbolNode;
            Assert.IsNotNull(T_1_4);
            Assert.AreEqual(1, T_1_4.Children.Count);
                
            var S_0_4_2 = S_0_4.Children[1] as IAndNode;
            Assert.IsNotNull(S_0_4_2);
            Assert.AreEqual(2, S_0_4_2.Children.Count);

            var A_0_1 = S_0_4_2.Children[0] as ISymbolNode;
            Assert.IsNotNull(A_0_1);
            Assert.AreEqual(2, A_0_1.Children.Count);

            var A_0_1_1 = A_0_1.Children[0] as IAndNode;
            Assert.IsNotNull(A_0_1_1);
            Assert.AreEqual(1, A_0_1_1.Children.Count);

            Assert.AreSame(a_0_1, A_0_1_1.Children[0]);

            var A_0_1_2 = A_0_1.Children[1] as IAndNode;
            Assert.IsNotNull(A_0_1_1);
            Assert.AreEqual(2, A_0_1_2.Children.Count);
            
            Assert.AreSame(A_0_1, A_0_1_2.Children[1]);

            var B_0_0 = A_0_1_2.Children[0] as ISymbolNode;
            Assert.IsNotNull(B_0_0);
            Assert.AreEqual(1, B_0_0.Children.Count);

            var B_0_0_1 = B_0_0.Children[0] as IAndNode;
            Assert.IsNotNull(B_0_0_1);
            Assert.AreEqual(1, B_0_0_1.Children.Count);

            var nullTerminal = B_0_0_1.Children[0] as ITerminalNode;
            Assert.IsNotNull(nullTerminal);
            Assert.AreEqual('\0', nullTerminal.Capture);

            var T_1_4_1 = T_1_4.Children[0] as IAndNode;
            Assert.IsNotNull(T_1_4_1);
            Assert.AreEqual(2, T_1_4_1.Children.Count);

            var T_1_3 = T_1_4_1.Children[0] as IIntermediateNode;
            Assert.IsNotNull(T_1_3);
            Assert.AreEqual(1, T_1_3.Children.Count);
            
            var b_3_4 = T_1_4_1.Children[1] as ITerminalNode;
            Assert.IsNotNull(b_3_4);
            Assert.AreEqual('b', b_3_4.Capture);

            var T_1_3_1 = T_1_3.Children[0] as IAndNode;
            Assert.IsNotNull(T_1_3_1);
            Assert.AreEqual(2, T_1_3_1.Children.Count);

            var b_1_2 = T_1_3_1.Children[0] as ITerminalNode;
            Assert.IsNotNull(b_1_2);
            Assert.AreEqual('b', b_1_2.Capture);

            var b_2_3 = T_1_3_1.Children[1] as ITerminalNode;
            Assert.IsNotNull(b_2_3);
            Assert.AreEqual('b', b_2_3.Capture);
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

            var S_0_1 = parseNode as ISymbolNode;
            Assert.IsNotNull(S_0_1);
            Assert.AreEqual(1, S_0_1.Children.Count);

            var S_0_1_1 = S_0_1.Children[0] as IAndNode;
            Assert.IsNotNull(S_0_1_1);
            Assert.AreEqual(1, S_0_1_1.Children.Count);

            var a_0_1 = S_0_1_1.Children[0] as ITerminalNode;
            Assert.IsNotNull(a_0_1);
            Assert.AreEqual('a', a_0_1.Capture);
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

            var S_0_1 = parser.ParseTree() as IInternalNode;
            Assert.IsNotNull(S_0_1);
            Assert.AreEqual(1, S_0_1.Children.Count);
            
            var S_0_1_1 = S_0_1.Children[0] as IAndNode;
            Assert.IsNotNull(S_0_1_1);
            Assert.AreEqual(1, S_0_1_1.Children.Count);

            var A_0_1 = S_0_1_1.Children[0] as IInternalNode;
            Assert.IsNotNull(A_0_1);
            Assert.AreEqual(1, A_0_1.Children.Count);

            var A_0_1_1 = A_0_1.Children[0] as IAndNode;
            Assert.IsNotNull(A_0_1_1);
            Assert.AreEqual(1, A_0_1_1.Children.Count);

            var a_0_1 = A_0_1_1.Children[0] as ITerminalNode;
            Assert.IsNotNull(a_0_1);
        }

        [TestMethod]
        public void Test_Parser_That_Leo_Items_Generate_Proper_Parse_Tree()
        {
            var grammar = new GrammarBuilder("S", p => p
                    .Production("S", r => r
                        .Rule("A"))
                    .Production("A", r => r
                        .Rule('a', "A")
                        .Rule('b')))
                .GetGrammar();
            const string input = "ab";
            var parser = new Parser(grammar);
            ParseInput(parser, input);

            /*  S_0_2 -> A_0_2
             *  A_0_2 -> a_0_1 A_1_2
             *  A_1_2 -> b_1_2
             */
            var S_0_2 = parser.ParseTree() as IInternalNode;
            Assert.IsNotNull(S_0_2);
            Assert.AreEqual(1, S_0_2.Children.Count);
            
            var S_0_4_1 = S_0_2.Children[0] as IAndNode;
            Assert.IsNotNull(S_0_4_1);

            var A_0_4 = S_0_4_1.Children[0] as IInternalNode;
            Assert.IsNotNull(A_0_4);
            Assert.AreEqual(2, A_0_4.Children.Count);
        }


        [TestMethod]
        public void Test_Parser_That_PassThrough_Recursive_Items_Creates_Virtual_Nodes()
        {
            var grammar = new GrammarBuilder("S", p => p
                .Production("S", r => r
                    .Rule("A"))
                .Production("A", r => r
                    .Rule('a', "B"))
                .Production("B", r => r
                    .Rule("A")
                    .Rule('b')))
            .GetGrammar();
            var parser = new Parser(grammar);
            const string input = "aaab";
            ParseInput(parser, input);

            var S_0_4 = parser.ParseTree() as IInternalNode;
            Assert.IsNotNull(S_0_4);
            Assert.AreEqual(1, S_0_4.Children.Count);

            var S_0_4_1 = S_0_4.Children[0] as IAndNode;
            Assert.IsNotNull(S_0_4_1);
            Assert.AreEqual(2, S_0_4_1.Children.Count);

            var A_0_4 = S_0_4_1.Children[0] as IInternalNode;
            Assert.IsNotNull(A_0_4);
        }

        private void ParseInput(Parser parser, string input)
        {
            foreach (var character in input)
                Assert.IsTrue(parser.Pulse(character));
            Assert.IsTrue(parser.IsAccepted());
        }
    }
}
