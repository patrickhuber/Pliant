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

        const string ToStringValue = "[0-9]";
        public override string ToString()
        {
            return ToStringValue;
        }

        public override bool Equals(object obj)
        {
            return obj is DigitTerminal;
        }

        public override int GetHashCode()
        {
            return ToStringValue.GetHashCode();
        }
    }
}
