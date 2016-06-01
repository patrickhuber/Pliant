using Pliant.Grammars;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Pliant.Builders.Fluent
{
    public class FluentProductionBuilder
    {
        IDictionary<INonTerminal, IList<IList<ISymbol>>> _productions;

        public FluentProductionBuilder()
        {
            _productions = new Dictionary<INonTerminal, IList<IList<ISymbol>>>();
        }

        public FluentProductionBuilder Production(INonTerminal leftHandSide, Action<FluentRuleBuilder> rules)
        {
            return new FluentProductionBuilder();
        }
    }
}