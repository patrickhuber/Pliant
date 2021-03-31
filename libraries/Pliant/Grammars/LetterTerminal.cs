using System.Collections.Generic;

namespace Pliant.Grammars
{
    public class LetterTerminal : BaseTerminal
    {
        private static readonly Interval[] _intervals = { new Interval('a', 'z'), new Interval('A', 'Z') };

        public override bool IsMatch(char character)
        {
            return char.IsLetter(character);
        }

        private const string ToStringValue = "[a-zA-Z]";

        public override string ToString()
        {
            return ToStringValue;
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            return obj is LetterTerminal;
        }

        public override int GetHashCode()
        {
            return ToStringValue.GetHashCode();
        }

        public override IReadOnlyList<Interval> GetIntervals()
        {
            return _intervals;
        }
    }
}
