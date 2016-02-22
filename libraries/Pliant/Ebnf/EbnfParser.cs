using Pliant.Ast;
using Pliant.Tree;
using System;

namespace Pliant.Ebnf
{
    public class EbnfParser
    {
        public EbnfDefinition Parse(string ebnf)
        {
            var grammar = new EbnfGrammar();
            var parseEngine = new ParseEngine(grammar, new ParseEngineOptions(optimizeRightRecursion: true));
            var parseInterface = new ParseInterface(parseEngine, ebnf);
            while (!parseInterface.EndOfStream())
            {
                if (!parseInterface.Read())
                    throw new Exception(
                        $"Unable to parse Ebnf. Error at position {parseInterface.Position}.");
            }
            if (!parseEngine.IsAccepted())
                throw new Exception(
                    $"Unable to parse Ebnf. Error at position {parseInterface.Position}");

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
