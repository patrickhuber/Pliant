using Pliant.Collections;
using System.Collections.Generic;
using System;
using Pliant.Utilities;

namespace Pliant.Grammars
{
    public class Grammar : IGrammar
    {
        protected ReadWriteList<ILexerRule> _ignores;
        protected ReadWriteList<IProduction> _productions;
        private Dictionary<INonTerminal, ReadWriteList<IProduction>> _productionIndex;
        private Dictionary<int, ReadWriteList<ILexerRule>> _ignoreIndex;
        private ISet<INonTerminal> _nullable;
        private Dictionary<INonTerminal, HashSet<IProduction>> _reverseLookup;

        private static readonly IProduction[] EmptyProductionArray = { };
        private static readonly ILexerRule[] EmptyLexerRuleArray = { };

        public Grammar()
        {
            _productions = new ReadWriteList<IProduction>();
            _ignores = new ReadWriteList<ILexerRule>();
            _productionIndex = new Dictionary<INonTerminal, ReadWriteList<IProduction>>();
            _ignoreIndex = new Dictionary<int, ReadWriteList<ILexerRule>>();
            _nullable = new HashSet<INonTerminal>();
            _reverseLookup = new Dictionary<INonTerminal, HashSet<IProduction>>();
        }

        public Grammar(
            INonTerminal start,
            IEnumerable<IProduction> productions,
            IEnumerable<ILexerRule> ignoreRules)
            : this()
        {
            Start = start;
            AddProductions(productions ?? EmptyProductionArray);
            AddIgnoreRules(ignoreRules ?? EmptyLexerRuleArray);
            FindNullableSymbols();
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
            if (!_productionIndex.ContainsKey(leftHandSide))
            {
                _productionIndex.Add(leftHandSide, new ReadWriteList<IProduction>());
            }
            _productionIndex[leftHandSide].Add(production);
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
                HashSet<IProduction> hashSet = null;
                if (!_reverseLookup.TryGetValue(nonTerminal, out hashSet))
                {
                    hashSet = new HashSet<IProduction>();
                    _reverseLookup.Add(nonTerminal, hashSet);
                }
                hashSet.Add(production);
            }
        }
        
        private void FindNullableSymbols()
        {
            // trace nullability through productions: http://cstheory.stackexchange.com/questions/2479/quickly-finding-empty-string-producing-nonterminals-in-a-cfg
            var nullableQueue = new Queue<INonTerminal>(_nullable);
            var productionSizes = new Dictionary<IProduction, int>();
            // foreach nullable symbol discovered in forming the reverse lookup
            while (nullableQueue.Count > 0)
            {
                var nonTerminal = nullableQueue.Dequeue();
                HashSet<IProduction> productionsContainingNonTerminal = null;
                if (_reverseLookup.TryGetValue(nonTerminal, out productionsContainingNonTerminal))
                {
                    foreach (var production in productionsContainingNonTerminal)
                    {
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
                        if (size == 0 && _nullable.Add(production.LeftHandSide))
                            nullableQueue.Enqueue(production.LeftHandSide);
                        productionSizes[production] = size;
                    }
                }
            }
        }

        private void AddIgnoreRule(ILexerRule lexerRule)
        {
            _ignores.Add(lexerRule);
            AddIgnoreRuletoIndex(lexerRule);
        }

        private void AddIgnoreRuletoIndex(ILexerRule lexerRule)
        {
            var key = HashCode.Compute(
                lexerRule.SymbolType.GetHashCode(),
                lexerRule.TokenType.Id.GetHashCode());
            if (!_ignoreIndex.ContainsKey(key))
                _ignoreIndex.Add(key, new ReadWriteList<ILexerRule>());
            _ignoreIndex[key].Add(lexerRule);
        }

        public INonTerminal Start { get; set; }

        public IReadOnlyList<IProduction> RulesFor(INonTerminal symbol)
        {
            ReadWriteList<IProduction> list;
            if (!_productionIndex.TryGetValue(symbol, out list))
                return EmptyProductionArray;
            return list;
        }

        public IEnumerable<IProduction> StartProductions()
        {
            return RulesFor(Start);
        }

        public bool IsNullable(INonTerminal nonTerminal)
        {
            return _nullable.Contains(nonTerminal);
        }        
    }
}