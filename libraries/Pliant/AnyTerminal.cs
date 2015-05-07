namespace Pliant
{
    public class AnyTerminal : ITerminal
    {
        public bool IsMatch(char character)
        {
            return true;
        }

        public SymbolType SymbolType
        {
            get { return SymbolType.Terminal; }
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
    }
}
