namespace Pliant.Grammars
{
    public abstract class BaseTerminal : Symbol, ITerminal
    {
        protected BaseTerminal() : base(SymbolType.Terminal)
        {
        }

        public abstract bool IsMatch(char character);

        public abstract Interval[] GetIntervals();
    }
}