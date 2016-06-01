using Pliant.Builders.Models;
using Pliant.Grammars;
using System;
using System.Collections.Generic;

namespace Pliant.Builders.Fluent
{
    public class FluentGrammarBuilder
    {
        public FluentGrammarBuilder()
        {
            Model = new GrammarModel();
        }
             
        public FluentGrammarBuilder Grammar(Action<FluentProductionBuilder> productions)
        {
            var fluentProductionBuilder = new FluentProductionBuilder();
            productions?.Invoke(fluentProductionBuilder);

            return this;
        }        

        public GrammarModel Model { get; private set; }
    }
}
