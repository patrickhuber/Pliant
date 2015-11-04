using Pliant.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Pliant.Grammars
{
    public class Grammar : IGrammar
    {
        protected ReadWriteList<ILexerRule> _ignores;
        protected ReadWriteList<IProduction> _productions;
        private IDictionary<INonTerminal, IList<IProduction>> _productionIndex;
        private IDictionary<int, IList<ILexerRule>> _ignoreIndex;

        private static readonly IProduction[] EmptyProductionArray = new IProduction[] { };
        private static readonly ILexerRule[] EmptyLexerRuleArray = new ILexerRule[] { };

        public Grammar()
        {
            _productions = new ReadWriteList<IProduction>();
            _ignores = new ReadWriteList<ILexerRule>();
            _productionIndex = new Dictionary<INonTerminal, IList<IProduction>>();
            _ignoreIndex = new Dictionary<int, IList<ILexerRule>>();
        }

        public Grammar(
            INonTerminal start,
            IProduction[] productions,
            ILexerRule[] ignoreRules)
            : this()
        {
            Start = start;
            AddProductions(productions ?? EmptyProductionArray);
            AddIgnoreRules(ignoreRules ?? EmptyLexerRuleArray);
        }

        private void AddIgnoreRules(ILexerRule[] ignoreRules)
        {
            foreach (var ignoreRule in ignoreRules)
                AddIgnoreRule(ignoreRule);
        }

        private void AddProductions(IProduction[] productions)
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

        public void AddProduction(IProduction production)
        {
            _productions.Add(production);
            AddProductionToIndex(production);
        }

        private void AddProductionToIndex(IProduction production)
        {
            var leftHandSide = production.LeftHandSide;
            if (!_productionIndex.ContainsKey(leftHandSide))
            {
                _productionIndex.Add(leftHandSide, new List<IProduction>());
            }
            _productionIndex[leftHandSide].Add(production);
        }

        public void AddIgnoreRule(ILexerRule lexerRule)
        {
            _ignores.Add(lexerRule);
            AddIgnoreRuletoIndex(lexerRule);
        }

        private void AddIgnoreRuletoIndex(ILexerRule lexerRule)
        {
            var key = HashUtil.ComputeHash(
                                lexerRule.SymbolType.GetHashCode(),
                                lexerRule.TokenType.Id.GetHashCode());
            if (!_ignoreIndex.ContainsKey(key))
                _ignoreIndex.Add(key, new List<ILexerRule>());
            _ignoreIndex[key].Add(lexerRule);
        }

        public INonTerminal Start { get; set; }

        public IEnumerable<IProduction> RulesFor(INonTerminal symbol)
        {
            IList<IProduction> list;
            if (!_productionIndex.TryGetValue(symbol, out list))
                return EmptyProductionArray;
            return list;
        }

        public IEnumerable<IProduction> StartProductions()
        {
            return RulesFor(Start);
        }        
    }
}
