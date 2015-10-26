using Pliant.Grammars;
using Pliant.Ast;
using System;
using System.Collections.Generic;

namespace Pliant.Regex
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
            return null;
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
