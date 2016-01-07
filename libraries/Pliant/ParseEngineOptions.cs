namespace Pliant
{
    public class ParseEngineOptions
    {
        public bool OptimizeRightRecursion { get; private set; }

        public ParseEngineOptions(bool optimizeRightRecursion = true)
        {
            OptimizeRightRecursion = optimizeRightRecursion;
        }
    }
}