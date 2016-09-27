using System;

namespace Pliant.Grammars
{
    public class NegationTerminal : BaseTerminal
    {
        public ITerminal InnerTerminal { get; private set; }

        public NegationTerminal(ITerminal innerTerminal)
        {
            InnerTerminal = innerTerminal;
        }

        public override bool IsMatch(char character)
        {
            return !InnerTerminal.IsMatch(character);
        }

        public override Interval[] GetIntervals()
        {
            throw new NotImplementedException();
        }
    }
}