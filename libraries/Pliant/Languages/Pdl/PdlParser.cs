using Pliant.Forest;
using Pliant.Runtime;
using Pliant.Tree;
using System;

namespace Pliant.Languages.Pdl
{
    public class PdlParser
    {
#pragma warning disable CC0091 // Use static method
        public PdlDefinition Parse(string ebnf)
        {
            var grammar = new PdlGrammar();
            var parseEngine = new ParseEngine(
                grammar, 
                new ParseEngineOptions(
                    optimizeRightRecursion: true,
                    loggingEnabled: false));
            var parseRunner = new ParseRunner(parseEngine, ebnf);
            while (!parseRunner.EndOfStream())
            {
                if (!parseRunner.Read())
                    throw new Exception(
                        $"Unable to parse Pdl. Error at line {parseRunner.Line}, column {parseRunner.Column}.");
            }
            if (!parseEngine.IsAccepted())
                throw new Exception(
                    $"Pdl parse not accepted. Error at line {parseRunner.Line}, column {parseRunner.Column}.");

            var parseForest = parseEngine.GetParseForestRootNode();

            var parseTree = new InternalTreeNode(
                    parseForest as IInternalForestNode,
                    new SelectFirstChildDisambiguationAlgorithm());

            var ebnfVisitor = new PdlVisitor();
            parseTree.Accept(ebnfVisitor);
            return ebnfVisitor.Definition;            
        }
#pragma warning restore CC0091 // Use static method
    }
}
