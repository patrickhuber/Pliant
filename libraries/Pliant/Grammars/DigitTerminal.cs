namespace Pliant.Grammars
{
    public class DigitTerminal : BaseTerminal
    {
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
    }
}