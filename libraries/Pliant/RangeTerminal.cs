using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pliant
{
    public class RangeTerminal : ITerminal
    {
        public char Start { get; private set; }
        public char End { get; private set; }

        public RangeTerminal(char start, char end)
        {
            Start = start;
            End = end;
        }

        public bool IsMatch(char character)
        {
            return Start <= character && character <= End;
        }

        public SymbolType SymbolType
        {
            get { return SymbolType.Terminal; }
        }

        public override string ToString()
        {
            return string.Format("[{0}-{1}]", Start, End);
        }

        public override int GetHashCode()
        {
            return Start.GetHashCode() ^ End.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var rangeTerminal = obj as RangeTerminal;
            if (rangeTerminal == null)
                return false;
            return rangeTerminal.End == End
                && rangeTerminal.Start == Start;
        }
    }
}
