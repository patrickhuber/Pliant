using Pliant.Grammars;
using Pliant.Nodes;
using System;
using System.Collections.Generic;

namespace Pliant
{
    public class RegexParser
    {
        private static IGrammar _grammar;

        static RegexParser()
        {
            _grammar = new RegexGrammar();
        }

        public IGrammar Parse(string regularExpression)
        {
            var parser = new Parser(_grammar);
            for (int i = 0; i < regularExpression.Length; i++)
            {
                var character = regularExpression[i];
                var result = parser.Pulse(character);
                if (!result)
                    throw new Exception(
                        string.Format(
                            "Invalid character in regular expression. Position: {0}", 
                            parser.Location));
            }
            return Compile(parser.ParseForest());
        }

        private IGrammar Compile(INode parseForest)
        {
            if (parseForest is IInternalNode)
            {
                var internalNode = parseForest as IInternalNode;

            }
            return null;
        }
        
        private static IEnumerable<IProduction> OneOrMany(INonTerminal identifier, ITerminal[] terminals)
        {
            var recursiveProductionSymbols = new List<ISymbol>();
            recursiveProductionSymbols.Add(identifier);
            recursiveProductionSymbols.AddRange(terminals);

            // NonTerminal -> NonTerminal Terminal | Terminal                    
            return new[] {
                    new Production(identifier, recursiveProductionSymbols.ToArray()),
                    new Production(identifier, terminals) };
        }

        private static IEnumerable<IProduction> ZeroOrMany(INonTerminal identifier, ITerminal[] terminals)
        {
            var recursiveProductionSymbols = new List<ISymbol>();
            recursiveProductionSymbols.Add(identifier);
            recursiveProductionSymbols.AddRange(terminals);

            // NonTerminal -> NonTerminal Terminal | <null>
            return new[]{
                new Production(identifier, recursiveProductionSymbols.ToArray()),
                new Production(identifier)};
        }

        private static IEnumerable<IProduction> ZeroOrOne(INonTerminal identifier, ITerminal[] terminals)
        {
            // NonTerminal -> Terminal | <null>
            return new[]{
                    new Production(identifier, terminals),
                    new Production(identifier) };
        }

        private static IEnumerable<IProduction> Once(INonTerminal identifier, ITerminal[] terminals)
        {
            // NonTerminal -> Terminal
            return new[]{
                new Production(identifier, terminals)};
        }
    }
}
