using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Regex;
using Pliant.Ast;
using Pliant.Tokens;
using Pliant.Builders;
using Pliant.Grammars;
using Pliant.Automata;

namespace Pliant.Tests.Unit.Ast
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

            var nodeVisitorStateManager = new NodeVisitorStateManager();
            var nodeVisitor = new LoggingNodeVisitor();
            var root = regexParseEngine.GetParseForestRoot();
            root.Accept(nodeVisitor);
            Assert.AreEqual(31, nodeVisitor.VisitLog.Count);           
        }

        [TestMethod]
        public void Test_NodeVisitor_That_Enumerates_All_Parse_Trees()
        {
            ProductionBuilder And = "AND",
                Panda = "Panda",
                AAn = "AAn",
                ShootsLeaves = "ShootsAndLeaves",
                EatsShootsLeaves = "EatsShootsLeaves"
                ;
            And.Definition = (_)'a' + 'n' + 'd';
            var and = new GrammarBuilder(And, new[] { And }).ToGrammar();

            Panda.Definition = (_)'p' + 'a' + 'n' + 'd' + 'a';
            var panda = new GrammarBuilder(Panda, new[] { Panda }).ToGrammar();

            AAn.Definition = (_)'a' | (_)'a' + 'n';
            var aAn = new GrammarBuilder(AAn, new[] { AAn }).ToGrammar();

            ShootsLeaves.Definition =
                (_) 's' + 'h' + 'o' + 'o' + 't' + 's'
                | (_)'l' + 'e' + 'a' + 'v' + 'e' + 's';
            var shootsLeaves = new GrammarBuilder(ShootsLeaves, new[] { ShootsLeaves }).ToGrammar();

            EatsShootsLeaves.Definition =
                (_) 'e' + 'a' + 't' + 's'
                | (_)'s' + 'h' + 'o' + 'o' + 't' + 's'
                | (_)'l' + 'e' + 'a' + 'v' + 'e' + 's';
            var eatsShootsLeaves = new GrammarBuilder(EatsShootsLeaves, new[] { EatsShootsLeaves }).ToGrammar();

            ProductionBuilder 
                S = "S", NP = "NP", VP = "VP", NN = "NN", 
                NNS = "NNS", DT = "DT", CC = "CC", VBZ="VBZ";

            S.Definition = 
                NP + VP + '.';
            NP.Definition = 
                NN 
                | NNS 
                | DT + NN 
                | NN + NNS 
                | NNS + CC + NNS;
            VP.Definition = VBZ + NP
                | VP + VBZ + NNS
                | VP + CC + VP
                | VP + VP + CC + VP
                | VBZ;
            CC.Definition = new GrammarLexerRule("CC", and);
            DT.Definition = new GrammarLexerRule("DT", aAn);
            NN.Definition = new GrammarLexerRule("NN", panda);
            NNS.Definition = new GrammarLexerRule("NNS", shootsLeaves);
            VBZ.Definition = new GrammarLexerRule("VBZ", eatsShootsLeaves);

            var grammar = new GrammarBuilder(
                S, 
                new[] { S, NP, VP, CC, DT, NN, NNS, VBZ }, 
                new[] { _whitespace })
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
