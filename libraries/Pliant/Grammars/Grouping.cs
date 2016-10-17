using System.Collections.Generic;

namespace Pliant.Grammars
{
    public class Grouping : IGrouping
    {
        private List<ISymbol> _items;

        public IReadOnlyList<ISymbol> Items { get { return _items; } }

        public Grouping(IReadOnlyList<ISymbol> items)
        {
            _items = new List<ISymbol>(items);
        }

        public virtual SymbolType SymbolType
        {
            get { return SymbolType.Grouping; }
        }
    }
}