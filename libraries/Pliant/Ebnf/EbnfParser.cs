using Pliant.Forest;
using Pliant.Tree;
using System;

namespace Pliant.Ebnf
{
    public class EbnfParser
    {
#pragma warning disable CC0091 // Use static method
        public EbnfDefinition Parse(string ebnf)
#pragma warning restore CC0091 // Use static method
        {
            var grammar = new EbnfGrammar();
            var parseEngine = new ParseEngine(grammar, new ParseEngineOptions(optimizeRightRecursion: true));
            var parseRunner = new ParseRunner(parseEngine, ebnf);
            while (!parseRunner.EndOfStream())
            {
                if (!parseRunner.Read())
                    throw new Exception(
                        $"Unable to parse Ebnf. Error at position {parseRunner.Position}.");
            }
            if (!parseEngine.IsAccepted())
                throw new Exception(
                    $"Unable to parse Ebnf. Error at position {parseRunner.Position}");

            var parseForest = parseEngine.GetParseForestRoot();
            var parseTree = new InternalTreeNode(
                parseForest as IInternalNode,
                new SinglePassNodeVisitorStateManager());
            var ebnfVisitor = new EbnfVisitor();
            parseTree.Accept(ebnfVisitor);
            return ebnfVisitor.Definition;
        }
    }
}
