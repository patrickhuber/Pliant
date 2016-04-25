using Pliant.Forest;
using Pliant.Tree;
using System;

namespace Pliant.RegularExpressions
{
    public class RegexParser
    {
        public Regex Parse(string regularExpression)
        {
            var grammar = new RegexGrammar();
            var parseEngine = new ParseEngine(grammar, new ParseEngineOptions(optimizeRightRecursion: true));
            var lexer = new Lexer(parseEngine, regularExpression);
            while (!lexer.EndOfStream())
            {
                if (!lexer.Read())
                    throw new Exception(
                        $"Unable to parse regular expression. Error at position {lexer.Position}.");
            }
            if (!parseEngine.IsAccepted())
                throw new Exception(
                    $"Error parsing regular expression. Error at position {lexer.Position}");

            var parseForest = parseEngine.GetParseForestRoot();
            var parseTree = new InternalTreeNode(
                parseForest as IInternalNode,
                new SinglePassNodeVisitorStateManager());

            var regexVisitor = new RegexVisitor();
            parseTree.Accept(regexVisitor);

            return regexVisitor.Regex;
        }
    }
}