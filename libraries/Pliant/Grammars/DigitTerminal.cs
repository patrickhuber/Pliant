namespace Pliant.Grammars
{
    public class DigitTerminal : ITerminal
    {
        public bool IsMatch(char character)
        {
            return char.IsDigit(character);
        }

        public SymbolType SymbolType
        {
            get { return SymbolType.Terminal; }
        }

        public override string ToString()
        {
            return "[0-9]";
        }

        public override bool Equals(object obj)
        {
            return obj is DigitTerminal;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
    }
}
