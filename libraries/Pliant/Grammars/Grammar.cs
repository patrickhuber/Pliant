using Pliant.Collections;
using System;
using System.Collections.Generic;
using Pliant.Utilities;

namespace Pliant.Grammars
{
    public class Grammar : IGrammar
    {
        private static readonly IProduction[] EmptyProductionArray = { };
        private static readonly ILexerRule[] EmptyLexerRuleArray = { };
        private static readonly DottedRule[] EmptyPredictionArray = { };

        protected readonly IndexedList<ILexerRule> _ignores;
        protected readonly IndexedList<ILexerRule> _trivia;
        protected readonly IndexedList<ILexerRule> _lexerRules;
        protected readonly IndexedList<IProduction> _productions;
        
        private readonly HashSet<ISymbol> _rightRecursiveSymbols;
        private readonly Dictionary<INonTerminal, List<IProduction>> _leftHandSideToProductions;
        private readonly UniqueList<INonTerminal> _transativeNullableSymbols;
        private readonly Dictionary<INonTerminal, UniqueList<IProduction>> _symbolsReverseLookup;
        private readonly IDottedRuleRegistry _dottedRuleRegistry;
        private readonly Dictionary<ISymbol, UniqueList<ISymbol>> _symbolPaths;

        public INonTerminal Start { get; private set; }

        public IReadOnlyList<IProduction> Productions { get { return _productions; } }

        public IReadOnlyList<ILexerRule> LexerRules { get { return _lexerRules; } }

        public IReadOnlyList<ILexerRule> Ignores { get { return _ignores; } }

        public IReadOnlyList<ILexerRule> Trivia { get { return _trivia; } }

        public IReadOnlyDottedRuleRegistry DottedRules { get { return _dottedRuleRegistry; } }

        public Grammar(
            INonTerminal start,
            IReadOnlyList<IProduction> productions,
            IReadOnlyList<ILexerRule> ignoreRules,
            IReadOnlyList<ILexerRule> triviaRules)
        {
            _productions = new IndexedList<IProduction>();
            _ignores = new IndexedList<ILexerRule>();
            _trivia = new IndexedList<ILexerRule>();

            _transativeNullableSymbols = new UniqueList<INonTerminal>();
            _symbolsReverseLookup = new Dictionary<INonTerminal, UniqueList<IProduction>>();
            _lexerRules = new IndexedList<ILexerRule>();
            _leftHandSideToProductions = new Dictionary<INonTerminal, List<IProduction>>();
            _dottedRuleRegistry = new DottedRuleRegistry();
            _symbolPaths = new Dictionary<ISymbol, UniqueList<ISymbol>>();
            
            Start = start;
            AddProductions(productions ?? EmptyProductionArray);
            AddIgnoreRules(ignoreRules ?? EmptyLexerRuleArray);
            AddTriviaRules(triviaRules ?? EmptyLexerRuleArray);

            _rightRecursiveSymbols = CreateRightRecursiveSymbols(_dottedRuleRegistry, _symbolPaths);
            FindNullableSymbols(_symbolsReverseLookup, _transativeNullableSymbols);
        }

        public int GetLexerRuleIndex(ILexerRule lexerRule)
        {
            return _lexerRules.IndexOf(lexerRule);
        }

        private void AddProductions(IReadOnlyList<IProduction> productions)
        {
            for (int p = 0; p < productions.Count; p++)
            {
                var production = productions[p];
                AddProduction(production);
            }
        }

        private void AddProduction(IProduction production)
        {
            _productions.Add(production);
            AddProductionToLeftHandSideLookup(production);

            if (production.IsEmpty)
            {
                _transativeNullableSymbols.Add(production.LeftHandSide);
            }

            var leftHandSide = production.LeftHandSide;
            var symbolPath = _symbolPaths.AddOrGetExisting(leftHandSide);

            for (var s = 0; s < production.RightHandSide.Count; s++)
            {
                var symbol = production.RightHandSide[s];
                if(symbol.SymbolType == SymbolType.LexerRule)
                    AddLexerRule(symbol as ILexerRule);
                RegisterDottedRule(production, s);
                RegisterSymbolPath(production, symbolPath, s);
                RegisterSymbolInReverseLookup(production, symbol);
            }
            RegisterDottedRule(production, production.RightHandSide.Count);
        }
        
        private void AddProductionToLeftHandSideLookup(IProduction production)
        {
            var leftHandSide = production.LeftHandSide;
            var indexedProductions = _leftHandSideToProductions.AddOrGetExisting(leftHandSide);
            indexedProductions.Add(production);
        }
        
        private void AddLexerRule(ILexerRule lexerRule)
        {
            _lexerRules.Add(lexerRule);
        }

        private void RegisterSymbolInReverseLookup(IProduction production, ISymbol symbol)
        {
            if (symbol.SymbolType == SymbolType.NonTerminal)
            {
                var nonTerminal = symbol as INonTerminal;
                var hashSet = _symbolsReverseLookup.AddOrGetExisting(nonTerminal);
                hashSet.Add(production);
            }
        }

        private static void RegisterSymbolPath(IProduction production, UniqueList<ISymbol> symbolPath, int s)
        {
            if (s < production.RightHandSide.Count)
            {
                var postDotSymbol = production.RightHandSide[s];
                symbolPath.AddUnique(postDotSymbol);
            }
        }

        private void RegisterDottedRule(IProduction production, int s)
        {
            var dottedRule = new DottedRule(production, s);
            _dottedRuleRegistry.Register(dottedRule);
        }

        private void AddIgnoreRules(IReadOnlyList<ILexerRule> ignoreRules)
        {
            for (int i = 0; i < ignoreRules.Count; i++)
            {
                var ignoreRule = ignoreRules[i];
                _ignores.Add(ignoreRule);
                _lexerRules.Add(ignoreRule);
            }
        }

        private void AddTriviaRules(IReadOnlyList<ILexerRule> triviaRules)
        {
            for (int i = 0; i < triviaRules.Count; i++)
            {
                var triviaRule = triviaRules[i];
                _trivia.Add(triviaRule);
                _lexerRules.Add(triviaRule);
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
                        for (var s = 0; s < production.RightHandSide.Count; s++)
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

        private HashSet<ISymbol> CreateRightRecursiveSymbols(
            IDottedRuleRegistry dottedRuleRegistry,
            Dictionary<ISymbol, UniqueList<ISymbol>> symbolPaths)
        {
            var hashSet = new HashSet<ISymbol>();
            for (var p = 0; p < _productions.Count; p++)
            {
                var production = _productions[p];
                var position = production.RightHandSide.Count;                
                var completed = dottedRuleRegistry.Get(production, position);
                var symbolPath = symbolPaths[production.LeftHandSide];

                for (var s = position; s > 0; s--)
                {
                    var preDotSymbol = production.RightHandSide[s - 1];
                    if (preDotSymbol.SymbolType != SymbolType.NonTerminal)
                        break;

                    var preDotNonTerminal = preDotSymbol as INonTerminal;
                    if (symbolPath.Contains(preDotNonTerminal))
                    {
                        hashSet.Add(production.LeftHandSide);
                        break;
                    }
                    if (!IsTransativeNullable(preDotNonTerminal))
                        break;
                }
            }
            return hashSet;
        }
        
        public bool IsTransativeNullable(INonTerminal nonTerminal)
        {
            return _transativeNullableSymbols.Contains(nonTerminal);
        }

        public bool IsNullable(INonTerminal nonTerminal)
        {
            List<IProduction> productionList;
            if (!_leftHandSideToProductions.TryGetValue(nonTerminal, out productionList))
                return true;
            if (productionList.Count > 0)
                return false;
            return productionList[0].RightHandSide.Count == 0;
        }

        public IReadOnlyList<IProduction> RulesContainingSymbol(INonTerminal nonTerminal)
        {
            UniqueList<IProduction> list;
            if (!_symbolsReverseLookup.TryGetValue(nonTerminal, out list))
                return EmptyProductionArray;
            return list;
        }

        public IReadOnlyList<IProduction> RulesFor(INonTerminal nonTerminal)
        {
            List<IProduction> list;
            if (!_leftHandSideToProductions.TryGetValue(nonTerminal, out list))
                return EmptyProductionArray;
            return list;
        }

        public IReadOnlyList<IProduction> StartProductions()
        {
            return RulesFor(Start);
        }

        public bool IsRightRecursive(ISymbol symbol)
        {
            return _rightRecursiveSymbols.Contains(symbol);
        }
    }
}
