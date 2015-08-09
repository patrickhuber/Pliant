using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Regex;
using Pliant.Nodes;
using Pliant.Tokens;
using Pliant.Builders;
using Pliant.Grammars;
using Pliant.Automata;

namespace Pliant.Tests.Unit.Nodes
{
    [TestClass]
    public class NodeVisitorTests
    {
        private readonly ILexerRule _whitespace;

        private ILexerRule CreateWhitespaceRule()
        {
            var start = new DfaState();
            var end = new DfaState(true);
            var startToEnd = new DfaTransition(new WhitespaceTerminal(), end);
            var endToEnd = new DfaTransition(new WhitespaceTerminal(), end);
            start.AddTransition(startToEnd);
            end.AddTransition(endToEnd);
            return new DfaLexerRule(start, new TokenType("whitespace"));
        }

        public NodeVisitorTests()
        {
            _whitespace = CreateWhitespaceRule();
        }

        [TestMethod]
        public void Test_NodeWalker_That_Walks_Simple_Regex()
        {
            var regexGrammar = new RegexGrammar();
            var regexParseEngine = new ParseEngine(regexGrammar);
            var regexParseInterface = new ParseInterface(regexParseEngine, @"[(]\d[)]");
            while (!regexParseInterface.EndOfStream())
            {
                if (!regexParseInterface.Read())
                    Assert.Fail("error parsing input at position {0}", regexParseInterface.Position);
            }
            Assert.IsTrue(regexParseEngine.IsAccepted());

            var nodeVisitor = new LoggingNodeVisitor();
            var nodeVisitorStateManager = new NodeVisitorStateManager();
            var root = regexParseEngine.GetRoot();
            root.Accept(nodeVisitor, nodeVisitorStateManager);
            Assert.AreEqual(32, nodeVisitor.VisitLog.Count);           
        }

        [TestMethod]
        public void Test_NodeVisitor_That_Enumerates_All_Parse_Trees()
        {
            var and = new GrammarBuilder("S")
                .Production("S", r => r
                    .Rule('a', 'n', 'd'))
                .ToGrammar();

            var panda = new GrammarBuilder("S")
                .Production("S", r => r
                    .Rule('p', 'a', 'n', 'd', 'a'))
                .ToGrammar();

            var aAn = new GrammarBuilder("S")
                .Production("S", r => r
                    .Rule('a')
                    .Rule('a', 'n'))
                .ToGrammar();

            var shootsLeaves = new GrammarBuilder("S")
                .Production("S", r => r
                    .Rule('s', 'h', 'o', 'o', 't', 's')
                    .Rule('l', 'e', 'a', 'v', 'e', 's'))
                .ToGrammar();

            var eatsShootsLeaves = new GrammarBuilder("S")
                .Production("S", r => r
                    .Rule('e', 'a', 't', 's')
                    .Rule('s', 'h', 'o', 'o', 't', 's')
                    .Rule('l', 'e', 'a', 'v', 'e', 's'))
                .ToGrammar();

            var grammar = new GrammarBuilder("S")
                .Production("S", r => r
                    .Rule("NP", "VP", '.'))
                .Production("NP", r => r
                    .Rule("NN")
                    .Rule("NNS")
                    .Rule("DT", "NN")
                    .Rule("NN", "NNS")
                    .Rule("NNS", "CC", "NNS"))
                .Production("VP", r => r
                    .Rule("VBZ", "NP")
                    .Rule("VP", "VBZ", "NNS")
                    .Rule("VP", "CC", "VP")
                    .Rule("VP", "VP", "CC", "VP")
                    .Rule("VBZ"))
                .Production("CC", r => r
                    .Rule(new GrammarLexerRule("CC", and)))
                .Production("DT", r => r
                    .Rule(new GrammarLexerRule("DT", aAn)))
                .Production("NN", r => r
                    .Rule(new GrammarLexerRule("NN", panda)))
                .Production("NNS", r => r
                    .Rule(new GrammarLexerRule("NNS", shootsLeaves)))
                .Production("VBZ", r => r
                    .Rule(new GrammarLexerRule("VBZ", eatsShootsLeaves)))
                .Ignore(_whitespace)
                .ToGrammar();

            var sentence = "a panda eats shoots and leaves.";

            var parseEngine = new ParseEngine(grammar);
            var parseInterface = new ParseInterface(parseEngine, sentence);

            while (!parseInterface.EndOfStream())
            {
                Assert.IsTrue(parseInterface.Read(), 
                    string.Format("Error parsing position: {0}", parseInterface.Position));
            }
            Assert.IsTrue(parseInterface.ParseEngine.IsAccepted());
        }    
    }
}
