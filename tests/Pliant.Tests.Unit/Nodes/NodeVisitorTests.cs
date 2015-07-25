using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Regex;
using Pliant.Nodes;
using Pliant.Tokens;
using Pliant.Builders;
using Pliant.Grammars;
using Pliant.Dfa;

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
            var startToEnd = new DfaEdge(new WhitespaceTerminal(), end);
            var endToEnd = new DfaEdge(new WhitespaceTerminal(), end);
            start.AddEdge(startToEnd);
            end.AddEdge(endToEnd);
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
            var and = new StringLiteralLexerRule("and", new TokenType("and"));
            var a = new StringLiteralLexerRule("a", new TokenType("a"));
            var an = new StringLiteralLexerRule("an", new TokenType("an"));
            var panda = new StringLiteralLexerRule("panda", new TokenType("panda"));
            var shoots = new StringLiteralLexerRule("shoots", new TokenType("shoots"));
            var eats = new StringLiteralLexerRule("eats", new TokenType("eats"));
            var leaves = new StringLiteralLexerRule("leaves", new TokenType("leaves"));

            var aAn = new GrammarBuilder("S")
                .Production("S", r => r
                    .Rule(a)
                    .Rule(an))
                .ToGrammar();

            var shootsLeaves = new GrammarBuilder("S")
                .Production("S", r => r
                    .Rule(shoots)
                    .Rule(leaves))
                .ToGrammar();

            var eatsShootsLeaves = new GrammarBuilder("S")
                .Production("S", r => r
                    .Rule(eats)
                    .Rule(shoots)
                    .Rule(leaves))
                .ToGrammar();

            var grammar = new GrammarBuilder("S")
                .Production("S", r=>r
                    .Rule("NP", "VP", '.'))
                .Production("NP", r=>r
                    .Rule("NN")
                    .Rule("NNS")
                    .Rule("DT", "NN")
                    .Rule("NN", "NNS")
                    .Rule("NNS", "CC", "NNS"))
                .Production("VP", r=>r
                    .Rule("VBZ", "NP")
                    .Rule("VP", "VBZ", "NNS")
                    .Rule("VP", "CC", "VP")
                    .Rule("VP", "VP", "CC", "VP")
                    .Rule("VBZ"))
                .Production("CC", r=>r
                    .Rule(and))
                .Production("DT", r=>r
                    .Rule(new GrammarLexerRule("DT", aAn)))
                .Production("NN", r=>r
                    .Rule(panda))
                .Production("NNS", r=>r
                    .Rule(new GrammarLexerRule("NNS", shootsLeaves)))
                .Production("VBZ", r=>r
                    .Rule(new GrammarLexerRule("VBZ", eatsShootsLeaves)))
                .Ignore(_whitespace)
                .ToGrammar();

            var sentence = "a panda eats shoots and leaves";

            var parseEngine = new ParseEngine(grammar);
            var parseInterface = new ParseInterface(parseEngine, sentence);

            while (!parseInterface.EndOfStream())
            {
                Assert.IsTrue(parseInterface.Read());
            }
            Assert.IsTrue(parseInterface.ParseEngine.IsAccepted());
        }    
    }
}
