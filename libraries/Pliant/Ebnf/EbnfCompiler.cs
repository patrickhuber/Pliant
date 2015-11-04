using Pliant.Grammars;
using Pliant.Ast;
using System;
using Pliant.Tree;

namespace Pliant.Ebnf
{
    /// <summary>
    /// A compiler turns input into a grammar format
    /// </summary>
    public class EbnfCompiler
    {
        public IGrammar Compile(string input)
        {
            var root = RunParse(input);
            var grammar = TranslateAst(root);
            return grammar;
        }

        private IGrammar TranslateAst(INode root)
        {
            var visitor = new EbnfVisitor();
            var internalRoot = root as IInternalNode;
            var internalTreeNode = new InternalTreeNode(internalRoot);
            internalTreeNode.Accept(visitor);
            return visitor.Grammar;
        }

        private static INode RunParse(string input)
        {
            var parseEngine = new ParseEngine(new EbnfGrammar());
            var parseInterface = new ParseInterface(parseEngine, input);

            while (!parseInterface.EndOfStream())
            {
                if (!parseInterface.Read())
                    throw new Exception(
                        string.Format("Error at position {0}", parseInterface.Position));
            }

            if (!parseInterface.ParseEngine.IsAccepted())
                throw new Exception(
                    "Unable to parse input. No acceptable parse paths found.");

            return parseInterface.ParseEngine.GetParseForestRoot();
        }
    }
}
