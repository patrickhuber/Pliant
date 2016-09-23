using Pliant.Collections;
using System.Collections.Generic;
using System;
using Pliant.Utilities;

namespace Pliant.Grammars
{
    public class Grammar : IGrammar
    {
        protected List<ILexerRule> _ignores;
        protected List<IProduction> _productions;
        private Dictionary<INonTerminal, List<IProduction>> _productionIndex;
        private Dictionary<int, List<ILexerRule>> _ignoreIndex;
        private UniqueList<INonTerminal> _nullable;
        private Dictionary<INonTerminal, UniqueList<IProduction>> _reverseLookup;
        private Dictionary<INonTerminal, UniqueList<PreComputedState>> _predictions;

        private static readonly IProduction[] EmptyProductionArray = { };
        private static readonly ILexerRule[] EmptyLexerRuleArray = { };
        private static readonly PreComputedState[] EmptyPredictionArray = { };        
        
        public Grammar(
            INonTerminal start,
            IEnumerable<IProduction> productions,
            IEnumerable<ILexerRule> ignoreRules)
        {
            _productions = new List<IProduction>();
            _ignores = new List<ILexerRule>();
            _productionIndex = new Dictionary<INonTerminal, List<IProduction>>();
            _ignoreIndex = new Dictionary<int, List<ILexerRule>>();
            _nullable = new UniqueList<INonTerminal>();
            _reverseLookup = new Dictionary<INonTerminal, UniqueList<IProduction>>();

            Start = start;
            AddProductions(productions ?? EmptyProductionArray);
            AddIgnoreRules(ignoreRules ?? EmptyLexerRuleArray);
            FindNullableSymbols(_reverseLookup, _nullable);

            _predictions = FindPredictions(_productionIndex, _nullable);
        }

        private void AddIgnoreRules(IEnumerable<ILexerRule> ignoreRules)
        {
            foreach (var ignoreRule in ignoreRules)
                AddIgnoreRule(ignoreRule);
        }

        private void AddProductions(IEnumerable<IProduction> productions)
        {
            foreach (var production in productions)
                AddProduction(production);
        }

        public IReadOnlyList<ILexerRule> Ignores
        {
            get { return _ignores; }
        }

        public IReadOnlyList<IProduction> Productions
        {
            get { return _productions; }
        }

        private void AddProduction(IProduction production)
        {
            _productions.Add(production);
            AddProductionToIndex(production);
            AddProductionToReverseLookup(production);
        }
        
        private void AddProductionToIndex(IProduction production)
        {
            var leftHandSide = production.LeftHandSide;
            var indexedProductions = _productionIndex.AddOrGetExisting(leftHandSide);
            indexedProductions.Add(production);
        }

        private void AddProductionToReverseLookup(IProduction production)
        {
            // get nullable nonterminals: http://cstheory.stackexchange.com/a/2493/32787
            if (production.IsEmpty)
                _nullable.Add(production.LeftHandSide);
            for(var s = 0; s< production.RightHandSide.Count; s++)
            {
                var symbol = production.RightHandSide[s];
                if (symbol.SymbolType != SymbolType.NonTerminal)
                    continue;
                var nonTerminal = symbol as INonTerminal;
                var hashSet = _reverseLookup.AddOrGetExisting(nonTerminal);
                hashSet.Add(production);
            }
        }
        
        private static void FindNullableSymbols(
            Dictionary<INonTerminal, UniqueList<IProduction>> reverseLookup, 
            UniqueList<INonTerminal> nullable)
        {
            // trace nullability through productions: http://cstheory.stackexchange.com/questions/2479/quickly-finding-empty-string-producing-nonterminals-in-a-cfg
            // I think this is Dijkstra's algorithm
            var nullableQueue = new Queue<INonTerminal>(nullable);
            var productionSizes = new Dictionary<IProduction, int>();
            // foreach nullable symbol discovered in forming the reverse lookup
            while (nullableQueue.Count > 0)
            {
                var nonTerminal = nullableQueue.Dequeue();
                UniqueList<IProduction> productionsContainingNonTerminal = null;
                if (reverseLookup.TryGetValue(nonTerminal, out productionsContainingNonTerminal))
                {
                    for (int p = 0; p < productionsContainingNonTerminal.Count; p++)
                    {
                        var production = productionsContainingNonTerminal[p];
                        var size = 0;
                        if (!productionSizes.TryGetValue(production, out size))
                        {
                            size = production.RightHandSide.Count;
                            productionSizes[production] = size;
                        }
                        for (var s=0; s< production.RightHandSide.Count; s++)
                        {
                            var symbol = production.RightHandSide[s];
                            if (symbol.SymbolType == SymbolType.NonTerminal 
                                && nonTerminal.Equals(symbol))
                                size--;
                        }
                        if (size == 0 && nullable.AddUnique(production.LeftHandSide))
                            nullableQueue.Enqueue(production.LeftHandSide);
                        productionSizes[production] = size;
                    }
                }
            }
        }

        private static Dictionary<INonTerminal, UniqueList<PreComputedState>> FindPredictions(
            Dictionary<INonTerminal, List<IProduction>> productionIndex,
            UniqueList<INonTerminal> nullable)
        {
            var predictions = new Dictionary<INonTerminal, UniqueList<PreComputedState>>();
            var queue = new Queue<PreComputedState>();

            // the production index contains all productions indexed by their left hand side 
            // symbol
            foreach (var symbolProductionList in productionIndex)
            {
                var symbol = symbolProductionList.Key;
                var productions = symbolProductionList.Value;
                var symbolPredictions = predictions.AddOrGetExisting(symbol);

                // initialize the predictions listed in the production index
                // these all start at position 0
                for (var p = 0; p < productions.Count; p++)
                {
                    var production = productions[p];
                    var state = new PreComputedState(production, 0);
                    if (symbolPredictions.AddUnique(state))
                        queue.Enqueue(state);
                }

                // compute the null closure over those productions above to get
                // all possible predicted states for symbol
                while (queue.Count > 0)
                {
                    var state = queue.Dequeue();

                    var isComplete = state.Position == state.Production.RightHandSide.Count;
                    if (isComplete)
                        continue;

                    var postDotSymbol = state.Production.RightHandSide[state.Position];
                    var isNonTerminal = postDotSymbol.SymbolType == SymbolType.NonTerminal;

                    if (!isNonTerminal)
                        continue;

                    var nonTerminalPostDotSymbol = postDotSymbol as INonTerminal;

                    List<IProduction> predictedProductions;
                    if (productionIndex.TryGetValue(nonTerminalPostDotSymbol, out predictedProductions))
                    {
                        for (var p = 0; p < predictedProductions.Count; p++)
                        {
                            var production = predictedProductions[p];
                            var prediction = new PreComputedState(production, 0);

                            if (symbolPredictions.AddUnique(prediction))
                                queue.Enqueue(prediction);
                        }
                    }

                    for (var s = state.Position; s < state.Production.RightHandSide.Count; s++)
                    {
                        var nullCheckSymbol = state.Production.RightHandSide[s];
                        if (nullCheckSymbol.SymbolType != SymbolType.NonTerminal)
                            break;
                        var nonTerminalNullCheckSymbol = nullCheckSymbol as INonTerminal;
                        if (!nullable.Contains(nonTerminalNullCheckSymbol))
                            break;

                        var aycockHorspoolState = new PreComputedState(state.Production, s + 1);
                        if (symbolPredictions.AddUnique(aycockHorspoolState))
                            queue.Enqueue(aycockHorspoolState);                            
                    }
                } 
                // queue is clear                
            }
            return predictions;
        }
        
        private void AddIgnoreRule(ILexerRule lexerRule)
        {
            _ignores.Add(lexerRule);
            AddIgnoreRuletoIndex(lexerRule);
        }

        private void AddIgnoreRuletoIndex(ILexerRule lexerRule)
        {
            var key = HashCode.Compute(
                ((int)lexerRule.SymbolType).GetHashCode(),
                lexerRule.TokenType.Id.GetHashCode());
            if (!_ignoreIndex.ContainsKey(key))
                _ignoreIndex.Add(key, new List<ILexerRule>());
            _ignoreIndex[key].Add(lexerRule);
        }

        public INonTerminal Start { get; set; }

        public IReadOnlyList<IProduction> RulesFor(INonTerminal symbol)
        {
            List<IProduction> list;
            if (!_productionIndex.TryGetValue(symbol, out list))
                return EmptyProductionArray;
            return list;
        }

        public IReadOnlyList<IProduction> RulesContainingSymbol(INonTerminal symbol)
        {
            UniqueList<IProduction> list;
            if (!_reverseLookup.TryGetValue(symbol, out list))
                return EmptyProductionArray;
            return list;
        }

        public IReadOnlyList<PreComputedState> PredictionsFor(INonTerminal symbol)
        {
            UniqueList<PreComputedState> list;
            if (!_predictions.TryGetValue(symbol, out list))
                return EmptyPredictionArray;
            return list;
        }

        public IReadOnlyList<IProduction> StartProductions()
        {
            return RulesFor(Start);
        }

        public bool IsNullable(INonTerminal nonTerminal)
        {
            return _nullable.Contains(nonTerminal);
        }

        public IReadOnlyList<INonTerminal> Nullable()
        {
            return _nullable;
        }    
    }
}