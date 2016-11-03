namespace Pliant.Runtime
{
    public class ParseEngineOptions : IParseEngineOptions
    {
        public bool OptimizeRightRecursion { get; private set; }

        public ParseEngineOptions(bool optimizeRightRecursion = true)
        {
            OptimizeRightRecursion = optimizeRightRecursion;
        }
    }
}