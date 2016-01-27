using Pliant.Collections;
using Pliant.Grammars;
using System;
using System.Collections.Generic;

namespace Pliant.Builders
{
    public class GrammarBuilder : IGrammarBuilder
    {
        private List<IProduction> _productions;
        private List<ILexerRule> _ignoreRules;
        public INonTerminal Start { get; set; }

        public GrammarBuilder(
            ProductionBuilder start,
            ProductionBuilder[] productionBuilder = null,
            ILexerRule[] ignore = null)
        {
            Start = start.LeftHandSide;

            _productions = new List<IProduction>();
            _ignoreRules = new List<ILexerRule>();

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

            var rules = production.Definition.Data;
            foreach (var rule in rules)
            {
                foreach (var builder in rule)
                {
                    if (builder is RuleBuilder) { }
                    else if (builder is ProductionBuilder)
                        TraverseAndAddProductions(
                            builder as ProductionBuilder, 
                            discoveredProductions);
                }
            }
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
                _productions.AddRange(builder.ToProductions());
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
                _productions.ToArray(),
                _ignoreRules.ToArray());
        }
    }
}