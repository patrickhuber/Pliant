using System.Collections.Generic;

namespace Pliant.Grammars
{
    public class Grouping : IGrouping
    {
        private List<ISymbol> _items;

        public IReadOnlyCollection<ISymbol> Items { get { return _items; } }

        public Grouping(IEnumerable<ISymbol> items)
        {
            _items = new List<ISymbol>(items);
        }

        public virtual SymbolType SymbolType
        {
            get { return SymbolType.Grouping; }
        }
    }
}