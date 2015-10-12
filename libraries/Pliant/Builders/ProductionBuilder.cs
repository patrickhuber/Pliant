using Pliant.Grammars;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pliant.Builders
{
    public class ProductionBuilder
    {
        public INonTerminal LeftHandSide { get; private set; }
        public IEnumerable<IEnumerable<ISymbol>> Alterations { get { return _alterations; } }

        private IList<IList<ISymbol>> _alterations;

        public ProductionBuilder(string leftHandSide)
        {
            LeftHandSide = new NonTerminal(leftHandSide);
            _alterations = new List<IList<ISymbol>>();
        }

        public AlterationBuilder Rule(params SymbolBuilder[] symbolBuilders)
        {
            var symbols = new List<ISymbol>();
            foreach (var symbolBuilder in symbolBuilders)
                symbols.Add(symbolBuilder.Symbol);
            _alterations.Add(symbols);
            var alterationBuilder = new AlterationBuilder(_alterations);            
            return alterationBuilder;
        }
    }
}
