namespace Pliant.Grammars
{
    public class AnyTerminal : BaseTerminal, ITerminal
    {
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
    }
}
