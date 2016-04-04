using System.Collections.Generic;

namespace Pliant.Grammars
{
    public class Repetition : Grouping, IRepetition
    {
        public Repetition(IEnumerable<ISymbol> items)
            : base(items) { }

        public override SymbolType SymbolType
        {
            get { return SymbolType.Repetition; }
        }
    }
}