using Pliant.Grammars;
using System;
using System.Collections.Generic;

namespace Pliant.Builders
{
    public class ProductionBuilder : BaseBuilder
    {
        public INonTerminal LeftHandSide { get; private set; }
        public override ISymbol Symbol { get { return LeftHandSide; } }
        public ProductionBuilder(string leftHandSide, string @namespace = "")
            : this(new NonTerminal(
                @namespace: @namespace, 
                @name: leftHandSide))
        {
        }
                
        public ProductionBuilder(INonTerminal leftHandSide)
        {
            LeftHandSide = leftHandSide;
        }
        
        public AlterationBuilder Rule(params SymbolBuilder[] symbolBuilders)
        {
            Definition = new RuleBuilder();
            foreach (var symbolBuilder in symbolBuilders)
                Definition.AddWithAnd(symbolBuilder);
            
            var alterationBuilder = new AlterationBuilder(Definition);
            return alterationBuilder;
        }

        public RuleBuilder Definition { get; set; }

        public IEnumerable<IProduction> ToProductions()
        {
            foreach (var builderList in Definition.Data)
            {
                var symbolList = new List<ISymbol>();
                foreach (var baseBuilder in builderList)
                {
                    if (baseBuilder is SymbolBuilder)
                    {
                        var symbolBuilder = baseBuilder as SymbolBuilder;
                        symbolList.Add(symbolBuilder.Symbol);
                    }
                    else if (baseBuilder is ProductionBuilder)
                    {
                        var productionBuilder = baseBuilder as ProductionBuilder;
                        symbolList.Add(productionBuilder.LeftHandSide);
                    }
                    else if (baseBuilder is ProductionReference)
                    {
                        var productionReference = baseBuilder as ProductionReference;
                        symbolList.Add(productionReference.Reference);
                    }
                }
                yield return new Production(LeftHandSide, symbolList);
            }
        }
        
        public static implicit operator ProductionBuilder(string name)
        {
            return new ProductionBuilder(name);
        }

        public static implicit operator ProductionBuilder(FullyQualifiedName name)
        {
            return new ProductionBuilder(name.Name, name.Namespace);
        }
    }
}