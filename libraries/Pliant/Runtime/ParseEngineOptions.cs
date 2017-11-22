namespace Pliant.Runtime
{
    public class ParseEngineOptions
    {
        public bool OptimizeRightRecursion { get; private set; }
        public bool LoggingEnabled { get; private set; }

        public ParseEngineOptions(bool optimizeRightRecursion = true, bool loggingEnabled = false)
        {
            OptimizeRightRecursion = optimizeRightRecursion;
            LoggingEnabled = loggingEnabled;
        }
    }
}