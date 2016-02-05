using Pliant.Grammars;
using System.Collections.Generic;

namespace Pliant.Builders
{
    public class ProductionBuilder : BaseBuilder
    {
        public INonTerminal LeftHandSide { get; private set; }

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

        public static implicit operator ProductionBuilder(string name)
        {
            return new ProductionBuilder(name);
        }

        public static implicit operator ProductionBuilder(FullyQualifiedName name)
        {
            return new ProductionBuilder(name.Name, name.Namespace);
        }

        public IEnumerable<IProduction> ToProductions()
        {
            foreach (var builderList in Definition.Data)
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
                yield return new Production(LeftHandSide, symbolList);
            }
        }
    }
}