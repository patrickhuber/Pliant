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
            bool productionBuilderBeenProcessed = !discoveredProductions.Add(production);
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

        public void AddIgnoreRule(ILexerRule lexerRule)
        {
            _ignoreRules.Add(lexerRule);
        }

        public IGrammar ToGrammar()
        {
            if (_productions.Count == 0)
                throw new Exception("no productions were found.");

            // PERF: Avoid Linq FirstOrDefault due to lambda allocation
            IProduction startProduction = null;
            foreach (var production in _productions)
            {
                if (Start == null)
                    Start = production.LeftHandSide;

                if (production.LeftHandSide.Equals(Start))
                {
                    startProduction = production;
                    break;
                }
            }

            if (startProduction == null)
                throw new Exception("no start production found for start symbol");

            var start = startProduction.LeftHandSide;

            return new Grammar(
                start,
                _productions,
                _ignoreRules);
        }
    }
}