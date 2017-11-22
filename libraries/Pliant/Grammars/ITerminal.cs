using System.Collections.Generic;

namespace Pliant.Grammars
{
    public interface ITerminal : ISymbol
    {
        bool IsMatch(char character);

        IReadOnlyList<Interval> GetIntervals();
    }
}