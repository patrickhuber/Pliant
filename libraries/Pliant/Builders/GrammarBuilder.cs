using Pliant.Collections;
using Pliant.Grammars;
using Pliant.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pliant.Builders
{
    public class GrammarBuilder : IGrammarBuilder
    {
        private IList<IProduction> _productions;
        private IList<ILexerRule> _ignoreRules;
        private string _start;

        public GrammarBuilder(string start)
        {
            _start = start;
            _productions = new List<IProduction>();
            _ignoreRules = new List<ILexerRule>();
        }

        public GrammarBuilder(
            ProductionBuilder start, 
            ProductionBuilder[] productionBuilder, 
            ILexerRule[] ignore)
            : this(start.LeftHandSide.Value)
        {
            foreach (var production in productionBuilder)
            {
                if (production.Definition == null)
                {
                    _productions.Add(new Production(production.LeftHandSide));
                    continue;
                }
                foreach (var list in production.Definition.Data)
                {
                    _productions.Add(GetProductionFromNameAndListOfSymbols(production.LeftHandSide.Value, list));
                }
            }
            if (!ignore.IsNullOrEmpty())
                foreach (var ignoreRule in ignore)
                    _ignoreRules.Add(ignoreRule);
        }

        public GrammarBuilder(ProductionBuilder start, ProductionBuilder[] productions)
            : this(start, productions, null)
        {
        }

        public IGrammar ToGrammar()
        {
            if (_start == null)
                throw new Exception("no start production specified");

            // PERF: Avoid Linq FirstOrDefault due to lambda allocation
            IProduction startProduction = null;
            foreach (var production in _productions)
                if (production.LeftHandSide.Value == _start)
                {
                    startProduction = production;
                    break;
                }

            if (startProduction == null)
                throw new Exception("no start production found for start symbol");

            var start = startProduction.LeftHandSide;

            return new Grammar(
                start, 
                _productions.ToArray(), 
                _ignoreRules.ToArray());
        }

        public IGrammarBuilder Production(string name, Action<IRuleBuilder> rules)
        {
            if (rules == null)
                _productions.Add(new Production(name));
            else
            {
                var ruleBuilder = new RuleBuilder();
                rules(ruleBuilder);

                var rightHandSide = new List<ISymbol>();
                foreach (var builderList in ruleBuilder.Data)
                {
                    var production = GetProductionFromNameAndListOfSymbols(name, builderList);
                    _productions.Add(production);
                }                
            }
            return this;
        }

        private static IProduction GetProductionFromNameAndListOfSymbols(string name, BaseBuilderList builderList)
        {
            var symbolList = new List<ISymbol>();
            foreach (var baseBuilder in builderList)
            {
                var symbolBuilder = baseBuilder as SymbolBuilder;
                if (symbolBuilder != null)                
                    symbolList.Add(symbolBuilder.Symbol);                
                else
                {
                    var productionBuilder = baseBuilder as ProductionBuilder;
                    if (productionBuilder != null)
                        symbolList.Add(productionBuilder.LeftHandSide);
                }
            }
            return new Production(name, symbolList.ToArray());
        }

        public IGrammarBuilder Ignore(ILexerRule lexerRule)
        {
            _ignoreRules.Add(lexerRule);
            return this;
        }
    }
}
