using System.Collections.Generic;

namespace Pliant.Grammars
{
    public abstract class BaseTerminal : Symbol, ITerminal
    {
        protected BaseTerminal() : base(SymbolType.Terminal)
        {
        }

        public abstract bool IsMatch(char character);

        public abstract IReadOnlyList<Interval> GetIntervals();
    }
}