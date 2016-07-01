using Pliant.Utilities;

namespace Pliant.Grammars
{
    public class RangeTerminal : BaseTerminal, ITerminal
    {
        public char Start { get; private set; }
        public char End { get; private set; }

        public RangeTerminal(char start, char end)
        {
            Start = start;
            End = end;
        }

        public override bool IsMatch(char character)
        {
            return Start <= character && character <= End;
        }

        public override string ToString()
        {
            return $"[{Start}-{End}]";
        }

        public override int GetHashCode()
        {
            return HashCode.Compute(
                Start.GetHashCode(),
                End.GetHashCode());
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