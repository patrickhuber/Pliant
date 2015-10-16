using Pliant.Grammars;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pliant.Builders
{
    public class ProductionBuilder : BaseBuilder
    {
        public INonTerminal LeftHandSide { get; private set; }
       
        public ProductionBuilder(string leftHandSide)
        {
            LeftHandSide = new NonTerminal(leftHandSide);
        }

        public AlterationBuilder Rule(params SymbolBuilder[] symbolBuilders)
        {
            Definition = new RuleBuilder();
            foreach (var symbolBuilder in symbolBuilders)
                Definition.Data[0].Add(symbolBuilder);
            
            var alterationBuilder = new AlterationBuilder(Definition);            
            return alterationBuilder;
        }
        
        public RuleBuilder Definition { get; set; }

        public static implicit operator ProductionBuilder(string name)
        {
            return new ProductionBuilder(name);
        }
    }
}
