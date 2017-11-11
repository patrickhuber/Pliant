using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pliant.Builders.Expressions;
using Pliant.Grammars;
using Pliant.Runtime;
using Pliant.Tokens;
using System.Collections.Generic;
using System.IO;
using System;

namespace Pliant.Tests.Common
{
    public class ParseTester
    {
        public IGrammar Grammar { get; private set; }
        public IParseEngine ParseEngine { get; private set; }
        public IParseRunner ParseRunner { get; private set; }

        public ParseTester(GrammarExpression expression)
            : this(expression.ToGrammar())
        {
        }

        public ParseTester(IGrammar grammar)
        {
            Grammar = grammar;
            ParseEngine = new ParseEngine(Grammar);
        }

        public ParseTester(IParseEngine parseEngine)
        {
            Grammar = parseEngine.Grammar;
            ParseEngine = parseEngine;
        }

        public void RunParse(string input)
        {
            ParseRunner = new ParseRunner(ParseEngine, input);
            InternalRunParse(ParseRunner);
        }
        
        public void RunParse(TextReader reader)
        {
            ParseRunner = new ParseRunner(ParseEngine, reader);
            InternalRunParse(ParseRunner);
        }

        private static void InternalRunParse(IParseRunner parseRunner)
        {
            while (!parseRunner.EndOfStream())            
                if (!parseRunner.Read())
                    Assert.Fail($"Parse Failed at Line: {parseRunner.Line}, Column: {parseRunner.Column}, Position: {parseRunner.Position}");
            
            if (!parseRunner.ParseEngine.IsAccepted())            
                Assert.Fail($"Parse was not accepted");            
        }

        public void RunParse(IReadOnlyList<IToken> tokens)
        {
            for (int i = 0; i < tokens.Count; i++)
                if (!ParseEngine.Pulse(tokens[i]))
                    Assert.Fail($"Parse Failed at Position {ParseEngine.Location}");

            if (!ParseEngine.IsAccepted())
                Assert.Fail($"Parse was not accepted");
        }
    }
}
