using System;

namespace Pliant.Grammars
{
    public class AnyTerminal : BaseTerminal
    {
        private static readonly Interval[] Interval = { new Interval(char.MinValue, char.MaxValue) };

        public override bool IsMatch(char character)
        {
            return true;
        }

        public override bool Equals(object obj)
        {
            var anyTerminal = obj as AnyTerminal;
            if (anyTerminal != null)
                return true;
            return false;
        }

        public override int GetHashCode()
        {
            return true.GetHashCode();
        }

        public override string ToString()
        {
            return ".";
        }

        public override Interval[] GetIntervals()
        {
            return Interval;
        }
    }
}