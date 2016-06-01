using Pliant.Grammars;

namespace Pliant.Builders.Models
{
    public abstract class SymbolModel
    {
        public abstract SymbolModelType ModelType { get; }

        public abstract ISymbol Symbol { get; }
    }
}