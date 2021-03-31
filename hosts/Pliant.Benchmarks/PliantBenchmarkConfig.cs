using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;

namespace Pliant.Benchmarks
{
    public class PliantBenchmarkConfig : ManualConfig
    {
        public PliantBenchmarkConfig()
        {
            AddDiagnoser(MemoryDiagnoser.Default);
        }
    }
}
