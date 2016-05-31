using Pliant.Collections;
using Pliant.Grammars;
using System;
using System.Collections.Generic;

namespace Pliant.Builders
{
    public class GrammarBuilder : IGrammarBuilder
    {
        private readonly HashSet<IProduction> _productions;
        private readonly HashSet<ILexerRule> _ignoreRules;
        public INonTerminal Start { get; set; }

        public GrammarBuilder(
            ProductionBuilder start,
            ProductionBuilder[] productionBuilder = null,
            ILexerRule[] ignore = null)
        {
            Assert.IsNotNull(start, nameof(start));
            Start = start.LeftHandSide;

            _productions = new HashSet<IProduction>();
            _ignoreRules = new HashSet<ILexerRule>();

            var shouldGetProductionsByTraversingStartSymbol = productionBuilder.IsNullOrEmpty();
            if (shouldGetProductionsByTraversingStartSymbol)
                AddProductionsFromStart(start);
            else
                AddProductions(productionBuilder);

            var shouldAddIgnoreRules = !ignore.IsNullOrEmpty();
            if (shouldAddIgnoreRules)
                AddIgnoreRules(ignore);
        }

        public GrammarBuilder()
        {
            _productions = new HashSet<IProduction>();
            _ignoreRules = new HashSet<ILexerRule>();
        }

        private void AddIgnoreRules(ILexerRule[] ignore)
        {
            foreach (var ignoreRule in ignore)
                _ignoreRules.Add(ignoreRule);
        }

        private void AddProductionsFromStart(ProductionBuilder start)
        {
            var hashSet = new HashSet<ProductionBuilder>();
            TraverseAndAddProductions(start, hashSet);
            AddProductions(hashSet);
        }

        private void TraverseAndAddProductions(
            ProductionBuilder production, 
            ISet<ProductionBuilder> discoveredProductions)
        {
            var productionBuilderBeenProcessed = !discoveredProductions.Add(production);
            if (productionBuilderBeenProcessed)
                return;

            var productionDefinition = production.Definition;
            if (productionDefinition == null)
                return;

            var rules = productionDefinition.Data;

            foreach (var rule in rules)
            {
                foreach (var builder in rule)
                {
                    if (builder is RuleBuilder) { }
                    else if (builder is ProductionBuilder)
                        TraverseAndAddProductions(
                            builder as ProductionBuilder,
                            discoveredProductions);
                    else if (builder is ProductionReference)
                    {
                        AddReferencedProduction(
                            builder as ProductionReference,
                            discoveredProductions);
                    }
                }
            }
        }

        private void AddReferencedProduction(
            ProductionReference productionReference, 
            ISet<ProductionBuilder> discoveredProductions)
        {
            foreach (var production in productionReference.Grammar.Productions)
                _productions.Add(production);
        }

        private void AddProductions(IEnumerable<ProductionBuilder> productionBuilder)
        {
            foreach (var production in productionBuilder)
            {
                AddProduction(production);
            }
        }

        public void AddProduction(ProductionBuilder builder)
        {
            if (builder.Definition == null)
                _productions.Add(new Production(builder.LeftHandSide));
            else
                foreach(var production in builder.ToProductions())
                    _productions.Add(production);
        }

        public void AddProduction(IProduction production)
        {
            _productions.Add(production);
        }

        public void AddIgnoreRule(ILexerRule lexerRule)
        {
            _ignoreRules.Add(lexerRule);
        }

        public IGrammar ToGrammar()
        {
            AssertAnyProductionsExist();

            var reachibiltyMatrix = GetReachibiltyMatrix();
            var startSymbolExists = Start != null;
            if (startSymbolExists)
                AssertStartProductionExistsForStartSymbol(reachibiltyMatrix);
            else
                Start = GetStartSymbolFromReachibiltyMatrix(reachibiltyMatrix);

            return new Grammar(
                Start,
                _productions,
                _ignoreRules);
        }

        private void AssertAnyProductionsExist()
        {
            if (_productions.Count == 0)
                throw new Exception("no productions were found.");
        }

        private IDictionary<INonTerminal, ISet<INonTerminal>> GetReachibiltyMatrix()
        {
            var reachibilityMatrix = new Dictionary<INonTerminal, ISet<INonTerminal>>();
            foreach (var production in _productions)
            {
                if (!reachibilityMatrix.ContainsKey(production.LeftHandSide))
                    reachibilityMatrix[production.LeftHandSide] = new HashSet<INonTerminal>();

                foreach (var symbol in production.RightHandSide)
                {
                    if (symbol.SymbolType != SymbolType.NonTerminal)
                        continue;

                    ISet<INonTerminal> set = null;
                    if (!reachibilityMatrix.TryGetValue(symbol as INonTerminal, out set))
                    {
                        set = new HashSet<INonTerminal>();
                        reachibilityMatrix[production.LeftHandSide] = set;
                    }

                    set.Add(production.LeftHandSide);
                }             
            }
            return reachibilityMatrix;
        }

        private void AssertStartProductionExistsForStartSymbol(IDictionary<INonTerminal, ISet<INonTerminal>> reachibilityMatrix)
        {
            if(!reachibilityMatrix.ContainsKey(Start))
                throw new Exception("no start production found for start symbol");
        }

        private static INonTerminal GetStartSymbolFromReachibiltyMatrix(IDictionary<INonTerminal, ISet<INonTerminal>> reachibiltyMatrix)
        {
            foreach (var leftHandSide in reachibiltyMatrix.Keys)
            {
                var symbolsReachableByLeftHandSide = reachibiltyMatrix[leftHandSide];
                if (symbolsReachableByLeftHandSide.Count == 0)
                    return leftHandSide;
            }
            return null;
        }

    }
}