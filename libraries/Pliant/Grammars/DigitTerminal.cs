using System;
using System.Collections.Generic;

namespace Pliant.Grammars
{
    public class DigitTerminal : BaseTerminal
    {
        private static readonly Interval[] _intervals = { new Interval('0', '9') };

        public override bool IsMatch(char character)
        {
            return char.IsDigit(character);
        }
        
        private const string ToStringValue = "[0-9]";

        public override string ToString()
        {
            return ToStringValue;
        }

        public override bool Equals(object obj)
        {
            if (((object)obj) == null)
                return false;
            return obj is DigitTerminal;
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