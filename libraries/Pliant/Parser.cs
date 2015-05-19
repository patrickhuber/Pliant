using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant
{
    public class Parser
    {
        private PulseRecognizer _pulseRecognizer;
        
        public Parser(IGrammar grammar)
        {
            _pulseRecognizer = new PulseRecognizer(grammar);
        }

        public bool Pulse(char token)
        {
            return _pulseRecognizer.Pulse(token);
        }

        public bool Pulse(IToken token)
        {
            return _pulseRecognizer.Pulse(token);
        }

        public IEnumerable<ILexerRule> GetLexerRules()
        {
            var chart = _pulseRecognizer.Chart;
            var earleySets = chart.EarleySets;
            var currentIndex = earleySets.Count - 1;
            var currentEarleySet = earleySets[currentIndex];
            var scanStates = currentEarleySet.Scans;
            
            foreach (var scanState in scanStates)
                yield return scanState
                    .DottedRule
                    .PostDotSymbol
                    .Value as ILexerRule;
        }

        public void Reset()
        {
            _pulseRecognizer.Reset();
        }

        public bool IsAccepted()
        {
            return _pulseRecognizer.IsAccepted();
        }

        public INode ParseForest()
        { 
            // thompson construction
            if (!IsAccepted())
                throw new Exception("Unable to parse expression.");
            var chart = _pulseRecognizer.Chart;
            var lastSet = chart.EarleySets[chart.Count - 1];
            var start = _pulseRecognizer.Grammar.Start;
            var completed = lastSet.Completions.First(x => x.Production.LeftHandSide.Equals(start));
            return completed.ParseNode;
        }

        private static IGrammar OneOrMany(INonTerminal identifier, ITerminal[] terminals)
        {
            var recursiveProductionSymbols = new List<ISymbol>();
            recursiveProductionSymbols.Add(identifier);
            recursiveProductionSymbols.AddRange(terminals);

            // NonTerminal -> NonTerminal Terminal | Terminal                    
            return new Grammar(
                identifier,
                new[]{
                    new Production(identifier, recursiveProductionSymbols.ToArray()),
                    new Production(identifier, terminals)},
                new IProduction[] { },
                new INonTerminal[] { });
        }

        private static IGrammar ZeroOrMany(INonTerminal identifier, ITerminal[] terminals)
        {
            var recursiveProductionSymbols = new List<ISymbol>();
            recursiveProductionSymbols.Add(identifier);
            recursiveProductionSymbols.AddRange(terminals);

            // NonTerminal -> NonTerminal Terminal | <null>
            return new Grammar(
                identifier,
                new[]{
                    new Production(identifier, recursiveProductionSymbols.ToArray()),
                    new Production(identifier)},
                new IProduction[] { },
                new INonTerminal[] { });
        }

        private static IGrammar ZeroOrOne(INonTerminal identifier, ITerminal[] terminals)
        {
            // NonTerminal -> Terminal | <null>
            return new Grammar(
                identifier,
                new[]{ 
                    new Production(identifier, terminals), 
                    new Production(identifier) },
                new IProduction[] { },
                new INonTerminal[] { });
        }

        private static IGrammar Once(INonTerminal identifier, ITerminal[] terminals)
        {
            // NonTerminal -> Terminal
            return new Grammar(
                identifier, new[]{ 
                    new Production(identifier, terminals)},
                new IProduction[] { },
                new INonTerminal[] { });
        }
    }
}
