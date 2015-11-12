using Pliant.Grammars;
using Pliant.Ast;
using System;
using System.Collections.Generic;
using Pliant.Tree;

namespace Pliant.RegularExpressions
{
    public class RegexParser 
    {
        public Regex Parse(string regularExpression)
        {
            var grammar = new RegexGrammar();
            var parseEngine = new ParseEngine(grammar);
            var parseInterface = new ParseInterface(parseEngine, regularExpression);
            while (!parseInterface.EndOfStream())
            {
                if (!parseInterface.Read())
                    throw new Exception(
                        $"Unable to parse regular expression. Error at position {parseInterface.Position}.");
            }
            if (!parseEngine.IsAccepted())
                throw new Exception($"Error parsing regular expression. Error at position {parseInterface.Position}");

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
