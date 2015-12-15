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

        public GrammarBuilder()
        {
            _productions = new List<IProduction>();
            _ignoreRules = new List<ILexerRule>();
        }

        public GrammarBuilder(
            ProductionBuilder start,
            ProductionBuilder[] productionBuilder,
            ILexerRule[] ignore)
            : this()
        {
            Start = start.LeftHandSide;
            AddProductions(productionBuilder);
            if (!ignore.IsNullOrEmpty())
                foreach (var ignoreRule in ignore)
                    _ignoreRules.Add(ignoreRule);
        }

        private void AddProductions(ProductionBuilder[] productionBuilder)
        {
            foreach (var production in productionBuilder)
            {
                AddProduction(production);
            }
        }

        public GrammarBuilder(ProductionBuilder start, ProductionBuilder[] productions)
            : this(start, productions, null)
        {
        }

        public void AddIgnoreRule(ILexerRule lexerRule)
        {
            _ignoreRules.Add(lexerRule);
        }

        public void AddProduction(ProductionBuilder builder)
        {
            if (builder.Definition == null)
                _productions.Add(new Production(builder.LeftHandSide));
            else
                _productions.AddRange(builder.ToProductions());
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