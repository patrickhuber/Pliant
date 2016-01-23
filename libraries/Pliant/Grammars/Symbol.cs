namespace Pliant.Grammars
{
    public abstract class Symbol : ISymbol
    {
        public SymbolType SymbolType { get; private set; }

        protected Symbol(SymbolType symbolType)
        {
            SymbolType = symbolType;
        }
    }
}