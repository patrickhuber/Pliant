namespace Pliant.Grammars
{
    public class NegationTerminal : BaseTerminal, ITerminal
    {
        private ITerminal _innerTerminal;

        public NegationTerminal(ITerminal innerTerminal)
        {
            _innerTerminal = innerTerminal;
        }

        public override bool IsMatch(char character)
        {
            return !_innerTerminal.IsMatch(character);
        }
    }
}