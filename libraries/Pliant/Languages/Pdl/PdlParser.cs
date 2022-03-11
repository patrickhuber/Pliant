using Pliant.Forest;
using Pliant.Runtime;
using Pliant.Tree;
using System;
using System.IO;

namespace Pliant.Languages.Pdl
{
    public class PdlParser
    {
#pragma warning disable CC0091 // Use static method
        public PdlDefinition Parse(string grammarText)
        {
            var parseEngine = CreateParseEngine();
            var parseRunner = new ParseRunner(parseEngine, grammarText);
            return RunParse(parseRunner);
        }
#pragma warning restore CC0091 // Use static method

        public PdlDefinition Parse(TextReader reader)
        {
            var parseEngine = CreateParseEngine();
            var parseRunner = new ParseRunner(parseEngine, reader);
            return RunParse(parseRunner);
        }

        private static PdlDefinition RunParse(IParseRunner parseRunner)
        {
            while (!parseRunner.EndOfStream())
            {
                if (!parseRunner.Read())
                    throw new Exception(
                        $"Unable to parse Pdl. Error at line {parseRunner.Line}, column {parseRunner.Column}.");
            }
            if (!parseRunner.ParseEngine.IsAccepted())
                throw new Exception(
                    $"Pdl parse not accepted. Error at line {parseRunner.Line}, column {parseRunner.Column}.");

            var parseForest = parseRunner.ParseEngine.GetParseForestRootNode();

            var parseTree = new InternalTreeNode(
                    parseForest as IInternalForestNode,
                    new SelectFirstChildDisambiguationAlgorithm());

            var ebnfVisitor = new PdlParseTreeVisitor();
            parseTree.Accept(ebnfVisitor);
            return ebnfVisitor.Definition;
        }

        private IParseEngine CreateParseEngine()
        {
            var grammar = new PdlGrammar();
            var parseEngine = new ParseEngine(
                grammar,
                new ParseEngineOptions(
                    optimizeRightRecursion: true,
                    loggingEnabled: false));
            return parseEngine;
        }
    }
}
