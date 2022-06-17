using Pliant.Collections;
using Pliant.Utilities;
using System.Collections;
using System.Collections.Generic;

namespace Pliant.Grammars
{
    public class Grammar : IGrammar
    {
        private static readonly IProduction[] EmptyProductionArray = { };
        private static readonly ILexerRule[] EmptyLexerRuleArray = { };        

        protected readonly IndexedList<ILexerRule> _ignores;
        protected readonly IndexedList<ILexerRule> _trivia;
        protected readonly IndexedList<ILexerRule> _lexerRules;
        protected readonly IndexedList<IProduction> _productions;
        
        private readonly HashSet<ISymbol> _rightRecursive;
        private readonly Dictionary<INonTerminal, List<IProduction>> _leftHandSideToProductions;        
        private readonly Dictionary<INonTerminal, UniqueList<IProduction>> _symbolsReverseLookup;
        private readonly IDottedRuleRegistry _dottedRuleRegistry;        

        private readonly HashSet<ISymbol> _transativeNullable;
        private readonly HashSet<ISymbol> _nullable;


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

            _symbolsReverseLookup = new Dictionary<INonTerminal, UniqueList<IProduction>>();
            _lexerRules = new IndexedList<ILexerRule>();
            _leftHandSideToProductions = new Dictionary<INonTerminal, List<IProduction>>();
            _dottedRuleRegistry = new DottedRuleRegistry();

            _nullable = new HashSet<ISymbol>();
            _transativeNullable = new HashSet<ISymbol>();
            _rightRecursive = new HashSet<ISymbol>();

            Start = start;
            AddProductions(productions ?? EmptyProductionArray);
            AddIgnoreRules(ignoreRules ?? EmptyLexerRuleArray);
            AddTriviaRules(triviaRules ?? EmptyLexerRuleArray);

            IdentifyNullableSymbols();
            IdentifyRightRecursiveSymbols();            
        }

        /// <summary>
        /// Implements the nullability check from the dragon book summarized here https://stackoverflow.com/a/19530120/516419
        /// </summary>
        private void IdentifyNullableSymbols()
        {
            var work = SharedPools.Default<Queue<IDottedRule>>().AllocateAndClear();
            var unprocessed = SharedPools.Default<Queue<IDottedRule>>().AllocateAndClear();

            for (var p = 0; p < _productions.Count; p++)
            {
                var production = _productions[p];
                if (production.IsEmpty)
                    _nullable.Add(production.LeftHandSide);
                var dottedRule = _dottedRuleRegistry.Get(production, 0);
                work.Enqueue(dottedRule);
            }
            
            var changes = 0;

            while (work.Count > 0 || unprocessed.Count > 0)
            {
                // if the work queue is empty
                if (work.Count == 0)
                {
                    // check if any changes were made
                    // if not exit, we can not process any more items
                    if (changes == 0)
                        break;
                    changes = 0;

                    // swap the queues
                    var temp = unprocessed;
                    unprocessed = work;
                    work = temp;
                }

                var dottedRule = work.Dequeue();

                // the dotted rule has been marked nullable already
                if (_transativeNullable.Contains(dottedRule.Production.LeftHandSide))
                {
                    changes++;
                    continue;
                }

                // the dotted rule is complete, therefore nullable
                if (dottedRule.IsComplete)
                {
                    _transativeNullable.Add(dottedRule.Production.LeftHandSide);
                    changes++;
                    continue;
                }

                // the dotted rule contains a terminal symbol
                if (dottedRule.PostDotSymbol.SymbolType != SymbolType.NonTerminal)
                {
                    changes++;
                    continue;
                }

                // the next symbol is nullable so enqueue the next dotted rule
                if (_transativeNullable.Contains(dottedRule.PostDotSymbol))
                {
                    var next = _dottedRuleRegistry.GetNext(dottedRule);
                    if (unprocessed.Contains(next))
                        continue;
                    unprocessed.Enqueue(next);
                    changes++;
                    continue;
                }

                // we can't process the item yet
                unprocessed.Enqueue(dottedRule);
            }

            SharedPools.Default<Queue<IDottedRule>>().Free(work);
            SharedPools.Default<Queue<IDottedRule>>().Free(unprocessed);
        }

        private void IdentifyRightRecursiveSymbols()
        {
            var work = SharedPools.Default<List<IDottedRule>>().AllocateAndClear();            

            for (var p = 0; p < _productions.Count; p++)
            {
                var production = _productions[p];

                for (var s = production.RightHandSide.Count; s > 0; s--)
                {
                    var dottedRule = _dottedRuleRegistry.Get(production, s);
                    var predotSymbol = dottedRule.PreDotSymbol;
                    if (predotSymbol.SymbolType != SymbolType.NonTerminal)
                        break;

                    work.Add(dottedRule);

                    // direct right recursion
                    if (production.LeftHandSide == predotSymbol)
                        _rightRecursive.Add(predotSymbol);

                    if (!IsNullable(predotSymbol as INonTerminal))
                        break;                    
                }
            }

            // no additional items are needed in the work set
            // collect the symbols into a vector
            var symbols = new IndexedList<ISymbol>();            
            for(var d =0; d< work.Count; d++)
            {
                var dottedRule = work[d];
                var lhs = dottedRule.Production.LeftHandSide;
                var pre = dottedRule.PreDotSymbol;

                if (!symbols.Contains(lhs))                
                    symbols.Add(lhs);
                if (!symbols.Contains(pre))
                    symbols.Add(pre);
            }

            // fill out the adjacency matrix
            var adjacency = new BitMatrix(symbols.Count);
            for (var d = 0; d < work.Count; d++)
            {
                var dottedRule = work[d];
                var lhs = dottedRule.Production.LeftHandSide;
                var pre = dottedRule.PreDotSymbol;
                adjacency[symbols.IndexOf(lhs)][symbols.IndexOf(pre)] = true;
            }

            // compute the transitive closure to detect any cycles
            var closure = adjacency.TransitiveClosure();

            // look for negative edges
            //
            // no cycles
            //   0 1 2
            // 0 * *
            // 1   * *
            // 2     *
            // 
            // cycles
            //   0 1 2
            // 0 * * *
            // 1   *
            // 2 * * *
            for (var d = 0; d < work.Count; d++)
            {
                var dottedRule = work[d];
                var lhs = dottedRule.Production.LeftHandSide;
                var pre = dottedRule.PreDotSymbol;

                // swap the lookup order because we are looking for back cycles
                if (closure[symbols.IndexOf(pre)][symbols.IndexOf(lhs)])
                    _rightRecursive.Add(lhs);
            }
            SharedPools.Default<List<IDottedRule>>().Free(work);
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

            var leftHandSide = production.LeftHandSide;

            for (var s = 0; s < production.RightHandSide.Count; s++)
            {
                var symbol = production.RightHandSide[s];
                if(symbol.SymbolType == SymbolType.LexerRule)
                    AddLexerRule(symbol as ILexerRule);
                RegisterDottedRule(production, s);
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
                
        public bool IsNullable(INonTerminal nonTerminal)
        {
            return _transativeNullable.Contains(nonTerminal);
        }

        public bool IsTransativeNullable(INonTerminal nonTerminal)
        {
            return _transativeNullable.Contains(nonTerminal);
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
            return _rightRecursive.Contains(symbol);
        }
    }
}
